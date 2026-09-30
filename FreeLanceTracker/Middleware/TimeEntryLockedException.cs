using Microsoft.AspNetCore.Diagnostics;

namespace FreeLanceTracker.Middleware;

public class TimeEntryLockedExceptionHandler(ILogger<TimeEntryLockedException> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not TimeEntryLockedException timeEntryLockedException) return false;
        logger.LogWarning(exception, "Resource is locked: {Message}", exception.Message);
        
        
        return true;
    }
}