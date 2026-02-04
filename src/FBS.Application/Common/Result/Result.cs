namespace FBS.Application.Common.Result;

public class Result
{
    public bool IsSuccess { get; }

    public string? Error { get; }

    public ErrorType ErrorType { get; }

    public Exception? Exception { get; }

    public static Result Success() => new(true);

    public static Result<TValue> Success<TValue>(TValue value)
        => new(value, true);

    public static Result Failure(string error, ErrorType errorType = ErrorType.Failure, Exception? exception = null)
        => new(false, error, errorType, exception);

    public static Result<TValue> Failure<TValue>(string error, ErrorType errorType = ErrorType.Failure, Exception? exception = null)
        => new(default, false, error, errorType, exception);


    public static Result NotFound(string error, Exception? exception = null)
        => Failure(error, ErrorType.NotFound, exception);

    public static Result<TValue> NotFound<TValue>(string error, Exception? exception = null)
        => Failure<TValue>(error, ErrorType.NotFound, exception);

    public static Result Conflict(string error, Exception? exception = null)
        => Failure(error, ErrorType.Conflict, exception);

    public static Result<TValue> Conflict<TValue>(string error, Exception? exception = null)
        => Failure<TValue>(error, ErrorType.Conflict, exception);

    public static Result ValidationError(string error, Exception? exception = null)
        => Failure(error, ErrorType.Validation, exception);

    public static Result<TValue> ValidationError<TValue>(string error, Exception? exception = null)
        => Failure<TValue>(error, ErrorType.Validation, exception);

    protected Result(bool isSuccess, string error = "", ErrorType errorType = ErrorType.None, Exception? exception = null)
    {
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Successful result cannot have an error");

        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failed result must have an error message");

        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
        Exception = exception;
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess ? _value!
        : throw new InvalidOperationException($"Cannot access value of a failed result. Error: {Error}");

    protected internal Result(
        TValue? value,
        bool isSuccess,
        string error = "",
        ErrorType errorType = ErrorType.None,
        Exception? exception = null)
        : base(isSuccess, error, errorType, exception)
    {
        _value = value;
    }
}