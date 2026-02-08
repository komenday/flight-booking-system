using FBS.Application.Common.Result;
using FBS.Domain.Common.Specifications;
using FBS.Domain.Repositories;
using FBS.Domain.SharedKernel;
using MediatR;

namespace FBS.Application.Queries.GetAvailableFlights;

public class GetAvailableFlightsQueryHandler : IRequestHandler<GetAvailableFlightsQuery, Result<List<FlightSummaryDto>>>
{
    private readonly IFlightRepository _flightRepository;

    public GetAvailableFlightsQueryHandler(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<Result<List<FlightSummaryDto>>> Handle(GetAvailableFlightsQuery request, CancellationToken cancellationToken)
    {
        var departure = Airport.From(request.DepartureAirport);
        var arrival = Airport.From(request.ArrivalAirport);

        var spec = new AvailableFlightsSpecification(departure, arrival, request.Date);
        var flights = await _flightRepository.GetAsync(spec, cancellationToken);

        var availableFlights = flights
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

        return Result.Success(availableFlights);
    }
}
