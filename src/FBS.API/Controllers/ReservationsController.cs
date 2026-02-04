using FBS.API.DTOs;
using FBS.Application.Commands.CancelReservation;
using FBS.Application.Commands.ConfirmReservation;
using FBS.Application.Commands.CreateReservation;
using FBS.Application.Common.Result;
using FBS.Application.Queries.GetReservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FBS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReservation(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetReservationQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateReservationCommand(
            request.FlightId,
            request.SeatNumber,
            new PassengerInfoDto(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Phone
            )
        );

        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetReservation),
                new { id = result.Value.ReservationId },
                result.Value)
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    [HttpPut("{id}/confirm")]
    public async Task<IActionResult> ConfirmReservation(Guid id, CancellationToken cancellationToken)
    {
        var command = new ConfirmReservationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelReservation(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelReservationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    private ObjectResult MapErrorToResponse(ErrorType errorType, string errorMessage)
    {
        return errorType switch
        {
            ErrorType.NotFound => NotFound(errorMessage),
            ErrorType.Validation => BadRequest(errorMessage),
            ErrorType.Conflict => Conflict(errorMessage),
            ErrorType.Unauthorized => Unauthorized(errorMessage),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, errorMessage),
            _ => BadRequest(errorMessage)
        };
    }
}
