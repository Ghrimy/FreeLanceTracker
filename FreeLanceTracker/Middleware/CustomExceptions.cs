namespace FreeLanceTracker.Middleware;

public abstract class AppException : Exception
{
    protected AppException(string message) : base(message) { }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message) { }
}

public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message) : base(message) { }
}

public class InvoiceLockedException : AppException
{
    public InvoiceLockedException(string message) : base(message) { }
}

public class TimeEntryLockedException : AppException
{
    public TimeEntryLockedException(string message) : base(message) { }
}