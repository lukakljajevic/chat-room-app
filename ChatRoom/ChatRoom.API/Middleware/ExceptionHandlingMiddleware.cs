using System.Net;
using System.Text.Json;

namespace ChatRoom.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (BadHttpRequestException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "Internal Server error");
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string errorMessage)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        response.StatusCode = (int)statusCode;
        var errorResponse = new ErrorResponse
        {
            Errors = errorMessage
        };
        logger.LogError(exception, "Exception: {Exception}", exception);

        var result = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(result);
    }
}

public record ErrorResponse
{
    public required string Errors { get; init; }
}