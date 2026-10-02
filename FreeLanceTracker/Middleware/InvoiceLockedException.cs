using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FreeLanceTracker.Middleware;

public class InvoiceLockedExceptionHandler(ILogger<InvoiceLockedExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not InvoiceLockedException invoiceLockedException) return false;

        logger.LogWarning(exception, "Resource is locked: {Message}", exception.Message);

        var problem = new ProblemDetails
        {
            Title = "Resource is locked",
            Status = StatusCodes.Status409Conflict,
            Detail = invoiceLockedException.Message
        };

        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}