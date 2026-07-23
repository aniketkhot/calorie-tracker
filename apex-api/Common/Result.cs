namespace ApexApi.Common;

// Result Pattern — returns success/failure explicitly instead of throwing for
// expected business failures (food not detected, plate not found, USDA has no
// match). Exceptions stay reserved for unexpected system failures. Calling code
// must check IsSuccess before touching Value, so a failure can't be silently
// ignored the way an unobserved exception sometimes can be.
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public ErrorCode? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(ErrorCode error, string message)
    {
        IsSuccess = false;
        Error = error;
        ErrorMessage = message;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(ErrorCode error, string message) => new(error, message);
}
