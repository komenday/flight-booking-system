namespace FBS.Application.Common.Result;

public enum ErrorType
{
    None,
    NotFound,
    Validation,
    Conflict,
    Failure,
    Unauthorized,
    Forbidden
}