using ProcrastiPlate.Contracts.Common;
using ProcrastiPlate.Core.Exceptions;

namespace ProcrastiPlate.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = context.Response;

        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            ValidationException validationException => new ErrorResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = validationException.Message,
                Details = "Validation failed"
            },
            NotFoundException notFoundException => new ErrorResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = notFoundException.Message,
                Details = "The requested resource was not found"
            },
            UnauthorizedException unauthorizedException => new ErrorResponse
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = unauthorizedException.Message,
                Details = "You do not have permission to perform this action"
            },
            _ => new ErrorResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "An internal server error occurred",
                Details = _env.IsDevelopment() ? exception.Message : null
            }
        };

        response.StatusCode = errorResponse.StatusCode;
        await response.WriteAsJsonAsync(errorResponse);
    }
}
