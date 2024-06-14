using ILogger = Serilog.ILogger;

namespace Diary.Api.Middlewares;

public class WriteAdressMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private bool _isStarted = false;

    public WriteAdressMiddleware(ILogger logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContent)
    {
        if (!_isStarted)
        {
            string url;
            if (httpContent.Request.IsHttps)
            {
                url = "https://";
            }
            else
            {
                url = "http://";
            }

            url += httpContent.Request.Host.ToString() +'/';
            Console.WriteLine("Now listening on: "+url);
            _isStarted = true;
        }

        await _next(httpContent);
    }
}