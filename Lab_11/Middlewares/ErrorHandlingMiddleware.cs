using System.Net;

namespace Lab_11.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)GetStatusCodeFromException(exception);
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = new
            {
                message = "An error occurred while processing your request.",
                detail = exception.Message
            }
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);

        return context.Response.WriteAsync(jsonResponse);
    }
    
    private HttpStatusCode GetStatusCodeFromException(Exception exception)
    {
        switch (exception)
        {
            case ArgumentNullException _:
            case ArgumentException _:
                return HttpStatusCode.BadRequest;

            case UnauthorizedAccessException _:
                return HttpStatusCode.Unauthorized;

            case KeyNotFoundException _:
                return HttpStatusCode.NotFound;

            case InvalidOperationException _:
                return HttpStatusCode.Conflict;

            default:
                return HttpStatusCode.InternalServerError;
        }
    }
}