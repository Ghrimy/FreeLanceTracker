namespace FreeLanceTracker.Middleware;

public abstract class AppException : Exception
{
    protected AppException(string message) : base(message) { }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message) { }
}

public class InvoiceLockedException : AppException
{
    public InvoiceLockedException(string message) : base(message) { }
}

public class TimeEntryLockedException : AppException
{
    public TimeEntryLockedException(string message) : base(message) { }
}

public class ValidationException : AppException
{
    public ValidationException(string message) : base(message) { }
}

public class GlobalException : AppException
{
    public GlobalException(string message) : base(message) { }
}
