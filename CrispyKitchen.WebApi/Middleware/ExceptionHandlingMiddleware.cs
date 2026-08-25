using CrispyKitchen.Application.Common.Exceptions;
using CrispyKitchen.Domain.Exceptions;
using FluentValidation;

namespace CrispyKitchen.WebApi.Middleware;

/// <summary>
/// One place that catches every unhandled exception and turns it into a
/// proper HTTP response — like a restaurant manager who steps in whenever
/// something goes wrong in the kitchen, so the customer never sees raw chaos,
/// just a clear, correct outcome (a 409, a 401, a 400 — never a mystery 500).
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            var (statusCode, message) = MapException(ex);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new { error = message });
        }
    }

    private static (int StatusCode, string Message) MapException(Exception ex) => ex switch
    {
        ConflictException => (StatusCodes.Status409Conflict, ex.Message),
        UnauthorizedException => (StatusCodes.Status401Unauthorized, ex.Message),
        ForbiddenException => (StatusCodes.Status403Forbidden, ex.Message),
        NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
        InvalidOrderTransitionException => (StatusCodes.Status400BadRequest, ex.Message),
        InsufficientStockException => (StatusCodes.Status409Conflict, ex.Message),
        ConcurrencyConflictException => (StatusCodes.Status409Conflict, ex.Message),

        ValidationException ve => (StatusCodes.Status400BadRequest, string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
    };
}