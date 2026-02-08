using FBS.API.DTOs;
using FBS.Application.Queries.GetAvailableFlights;
using FBS.Application.Queries.GetFlightByNumber;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FBS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FlightsController(IMediator mediator) : ApiControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<FlightSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchFlights([FromQuery] SearchFlightsRequest searchFlightsRequest, CancellationToken cancellationToken)
    {
        var query = new GetAvailableFlightsQuery(
            searchFlightsRequest.DepartureAirport,
            searchFlightsRequest.ArrivalAirport,
            searchFlightsRequest.Date
        );

        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }

    [HttpGet("{flightNumber}")]
    [ProducesResponseType(typeof(FlightDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetFlightByNumber(string flightNumber, CancellationToken cancellationToken)
    {
        var query = new GetFlightByNumberQuery(flightNumber);
        var result = await _mediator.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : MapErrorToResponse(result.ErrorType, result.Error!);
    }
}