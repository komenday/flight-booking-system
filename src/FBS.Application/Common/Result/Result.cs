namespace FBS.Application.Common.Result;

public class Result
{
    public bool IsSuccess { get; }

    public string? Error { get; }

    public Exception? Exception { get; }

    public static Result Success() => new(true);

    public static Result Failure(string error, Exception? exception = null)
        => new(false, error, exception);

    public static Result<TValue> Success<TValue>(TValue value)
        => new(value, true);

    public static Result<TValue> Failure<TValue>(string error, Exception? exception = null)
        => new(default, false, error, exception);

    protected Result(bool isSuccess, string? error = null, Exception? exception = null)
    {
        if (isSuccess && error is not null)
            throw new InvalidOperationException("Successful result cannot have an error");

        if (!isSuccess && error is null)
            throw new InvalidOperationException("Failed result must have an error message");

        IsSuccess = isSuccess;
        Error = error;
        Exception = exception;
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot access value of a failed result. Error: {Error}");

    protected internal Result(
        TValue? value,
        bool isSuccess,
        string? error = null,
        Exception? exception = null)
        : base(isSuccess, error, exception)
    {
        _value = value;
    }

    public static implicit operator Result<TValue>(TValue value) => Success(value);
}