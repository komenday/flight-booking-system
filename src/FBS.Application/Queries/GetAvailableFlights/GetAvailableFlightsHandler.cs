using FBS.Domain.Repositories;
using FBS.Domain.SharedKernel;
using MediatR;

namespace FBS.Application.Queries.GetAvailableFlights;

public class GetAvailableFlightsHandler : IRequestHandler<GetAvailableFlightsQuery, List<FlightSummaryDto>>
{
    private readonly IFlightRepository _flightRepository;

    public GetAvailableFlightsHandler(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<List<FlightSummaryDto>> Handle(GetAvailableFlightsQuery request, CancellationToken cancellationToken)
    {
        var departure = Airport.From(request.DepartureAirport);
        var arrival = Airport.From(request.ArrivalAirport);

        var flights = await _flightRepository.GetAvailableFlightsAsync(departure, arrival, request.Date, cancellationToken);

        return flights
            .Where(f => !f.IsFull())
            .Select(f => new FlightSummaryDto(
                f.Id.Value,
                f.Number.Value,
                f.Departure.IataCode,
                f.Arrival.IataCode,
                f.DepartureTime,
                f.GetAvailableSeatsCount()
            ))
            .ToList();
    }
}
