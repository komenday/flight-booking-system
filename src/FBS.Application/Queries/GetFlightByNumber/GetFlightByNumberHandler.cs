using FBS.Domain.Flight;
using FBS.Domain.Repositories;
using MediatR;

namespace FBS.Application.Queries.GetFlightByNumber;

public class GetFlightByNumberHandler : IRequestHandler<GetFlightByNumberQuery, FlightDetailsDto?>
{
    private readonly IFlightRepository _flightRepository;

    public GetFlightByNumberHandler(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<FlightDetailsDto?> Handle(GetFlightByNumberQuery request, CancellationToken cancellationToken)
    {
        var flightNumber = FlightNumber.From(request.FlightNumber);

        var flight = await _flightRepository.GetByFlightNumberAsync(flightNumber, cancellationToken);

        if (flight is null)
            return null;

        return new FlightDetailsDto(
            flight.Id.Value,
            flight.Number.Value,
            flight.Departure.IataCode,
            flight.Arrival.IataCode,
            flight.DepartureTime,
            flight.Seats.Count,
            flight.GetAvailableSeatsCount(),
            flight.Seats.Select(s => new SeatDto(
                s.Number.Value,
                s.Type.ToString(),
                s.IsAvailable
            )).ToList()
        );
    }
}
