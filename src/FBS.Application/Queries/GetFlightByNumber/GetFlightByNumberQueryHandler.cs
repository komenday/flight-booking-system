using FBS.Application.Common.Result;
using FBS.Domain.Common.Specifications;
using FBS.Domain.Flight;
using FBS.Domain.Repositories;
using MediatR;

namespace FBS.Application.Queries.GetFlightByNumber;

public class GetFlightByNumberQueryHandler : IRequestHandler<GetFlightByNumberQuery, Result<FlightDetailsDto>>
{
    private readonly IFlightRepository _flightRepository;

    public GetFlightByNumberQueryHandler(IFlightRepository flightRepository)
    {
        _flightRepository = flightRepository;
    }

    public async Task<Result<FlightDetailsDto>> Handle(GetFlightByNumberQuery request, CancellationToken cancellationToken)
    {
        var flightNumber = FlightNumber.From(request.FlightNumber);

        var spec = new FlightByNumberWithSeatsSpecification(flightNumber);
        var flight = await _flightRepository.GetFirstOrDefaultAsync(spec, cancellationToken);

        if (flight is null)
        {
            return Result.NotFound<FlightDetailsDto>($"Flight with flight number {request.FlightNumber} was not found");
        }

        var flightDetails = new FlightDetailsDto(
            flight.Id.Value,
            flight.Number.Value,
            flight.Departure.IataCode,
            flight.Arrival.IataCode,
            flight.DepartureTime,
            flight.Seats.Count,
            flight.GetAvailableSeatsCount(),
            flight.Seats.Select(s => new SeatDto(
                s.Number.Value,
                s.IsAvailable
            )).ToList()
        );

        return Result.Success(flightDetails);
    }
}
