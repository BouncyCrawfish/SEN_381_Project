namespace CivicConnect.Common;

public enum ErrorType { Validation, NotFound, Conflict, Unauthorized, Forbidden, Locked }

/// <summary>A user-presentable failure. Never put secrets or stack traces in Message.</summary>
public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Validation);
}

/// <summary>Outcome of an operation with no return value (PED section 5.7: Result-based error handling).</summary>
public sealed class Result
{
    private Result(bool isSuccess, Error error) { IsSuccess = isSuccess; Error = error; }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

/// <summary>Outcome of an operation that returns a value on success.</summary>
public sealed class Result<T>
{
    private readonly T? _value;

    private Result(T? value, bool isSuccess, Error error)
    {
        _value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot read Value of a failed result.");

    public static Result<T> Success(T value) => new(value, true, Error.None);
    public static Result<T> Failure(Error error) => new(default, false, error);
}
