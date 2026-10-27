using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FreeLanceTracker.Middleware;

public class TimeEntryLockedExceptionHandler(ILogger<TimeEntryLockedExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not TimeEntryLockedException timeEntryLockedException) return false;
        logger.LogWarning(exception, "Resource is locked: {Message}", exception.Message);

        var problem = new ProblemDetails
        {
            Title = "Resource is locked",
            Status = StatusCodes.Status409Conflict,
            Detail = timeEntryLockedException.Message
        };

        context.Response.StatusCode = problem.Status.Value;
        await context.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}