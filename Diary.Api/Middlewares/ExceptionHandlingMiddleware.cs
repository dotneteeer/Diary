using System.Net;
using System.Text;
using Diary.Domain.Result;
using Newtonsoft.Json;
using IFormattable = System.IFormattable;
using ILogger = Serilog.ILogger;

namespace Diary.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            using (var swapStream = new MemoryStream())
            {
                var originalResponseBody = httpContext.Response.Body;

                httpContext.Response.Body = swapStream;

                await _next(httpContext);

                swapStream.Seek(0, SeekOrigin.Begin);
                string responseBody = new StreamReader(swapStream).ReadToEnd();
                swapStream.Seek(0, SeekOrigin.Begin);

                switch (httpContext.Response.StatusCode)
                {
                    case (int)HttpStatusCode.BadRequest:
                        dynamic data = JsonConvert.DeserializeObject(responseBody);
                        string errorMessage = data.errorMessage;
                        _logger.Warning(new ArgumentException("Bad request: " + errorMessage), errorMessage);
                        break;
                    case (int)HttpStatusCode.NotFound:
                        httpContext.Response.ContentType = "text/html";
                        const string pageNotFoundImage = "<img style=\"width: 100%; height: 100%;\" src=\"https://colorlib.com/wp/wp-content/uploads/sites/2/404-error-template-3.png\" alt=\"404. We are sorry, but the page you requested was not found.\"/>";
                        await WriteToStream(swapStream, pageNotFoundImage);
                        break;
                }

                await swapStream.CopyToAsync(originalResponseBody);
                httpContext.Response.Body = originalResponseBody;
            }
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        _logger.Error(exception, exception.Message);

        var errorMessage = exception.Message;

        var response = exception switch
        {
            UnauthorizedAccessException _ => new BaseResult
            {
                ErrorMessage = errorMessage,
                ErrorCode = (int)HttpStatusCode.Unauthorized
            },

            _ => new BaseResult
            {
                ErrorMessage = "Internal server error: " + errorMessage, //might be hardcode
                ErrorCode = (int)HttpStatusCode.InternalServerError
            }
        };


        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)response.ErrorCode;
        await httpContext.Response.WriteAsJsonAsync(response);
    }
    private async Task WriteToStream(MemoryStream stream, string message)
     {
         var buffer = Encoding.UTF8.GetBytes(message);
         await stream.WriteAsync(buffer, 0, buffer.Length);
         stream.Seek(0, SeekOrigin.Begin);
     }
}

