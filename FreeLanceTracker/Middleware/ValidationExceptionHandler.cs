using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Middleware;

public class ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not ValidationException validationException) return false;
        logger.LogWarning(exception, "Validation failed: {Message}", exception.Message);

        var problem = new ProblemDetails
        {
            Title = "Validation Failed",
            Status = StatusCodes.Status400BadRequest,
            Detail = validationException.Message
        };

        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}