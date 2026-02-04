using FBS.Application.Common.Result;
using FBS.Application.Queries.GetAvailableFlights;
using FBS.Application.Queries.GetFlightByNumber;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FBS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("search")]
    public async Task<IActionResult> SearchFlights([FromQuery] string departureAirport, [FromQuery] string arrivalAirport, [FromQuery] DateTime date, CancellationToken cancellationToken)
    {
        var query = new GetAvailableFlightsQuery(
            departureAirport,
            arrivalAirport,
            date
        );

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    [HttpGet("{flightNumber}")]
    public async Task<IActionResult> GetFlightByNumber(string flightNumber, CancellationToken cancellationToken)
    {
        var query = new GetFlightByNumberQuery(flightNumber);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    private ObjectResult MapErrorToResponse(ErrorType errorType, string errorMessage)
    {
        return errorType switch
        {
            ErrorType.NotFound => NotFound(errorMessage),
            ErrorType.Validation => BadRequest(errorMessage),
            ErrorType.Conflict => Conflict(errorMessage),
            _ => BadRequest(errorMessage)
        };
    }
}