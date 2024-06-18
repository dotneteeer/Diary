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
            await _next(httpContext);

            switch (httpContext.Response.StatusCode)
            {
                case (int)HttpStatusCode.NotFound:
                    httpContext.Response.ContentType = "text/plain";
                    var message = $"{(int)HttpStatusCode.NotFound} {nameof(HttpStatusCode.NotFound)}\nPlease check URL";
                    await httpContext.Response.WriteAsync(message);
                    break;
            }
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {

        var errorMessage = exception.Message;
        
        _logger.Error(exception, errorMessage);
        

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

        
        httpContext.Response.ContentType= "application/json";
        httpContext.Response.StatusCode = (int)response.ErrorCode;
        await httpContext.Response.WriteAsJsonAsync(response);
    }
}