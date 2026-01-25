using FBS.Domain.Flight;

namespace FBS.Application.Exceptions;

public class FlightNotFoundException(FlightId flightId)
    : NotFoundException($"Flight with ID {flightId} was not found")
{
    public FlightId FlightId { get; } = flightId;
}
