using FBS.API.DTOs;
using FBS.Application.Commands.CancelReservation;
using FBS.Application.Commands.ConfirmReservation;
using FBS.Application.Commands.CreateReservation;
using FBS.Application.Queries.GetReservation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FBS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController(IMediator mediator) : ApiControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("get/{id}")]
    [ProducesResponseType(typeof(ReservationDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReservation(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetReservationQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(CreateReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateReservationCommand(
            request.FlightNumber,
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

    [HttpPut("confirm/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmReservation(Guid id, CancellationToken cancellationToken)
    {
        var command = new ConfirmReservationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    [HttpDelete("cancel/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelReservation(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelReservationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }
}
