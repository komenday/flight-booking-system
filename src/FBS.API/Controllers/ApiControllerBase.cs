using FBS.Application.Common.Result;
using Microsoft.AspNetCore.Mvc;

namespace FBS.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ObjectResult MapErrorToResponse(ErrorType errorType, string errorMessage)
    {
        return errorType switch
        {
            ErrorType.NotFound => NotFound(errorMessage),
            ErrorType.Validation => BadRequest(errorMessage),
            ErrorType.Conflict => Conflict(errorMessage),
            ErrorType.Unauthorized => Unauthorized(errorMessage),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, errorMessage),
            ErrorType.Failure => StatusCode(StatusCodes.Status500InternalServerError, errorMessage),
            _ => BadRequest(errorMessage)
        };
    }
}