using FBS.Domain.Common.Exceptions;

namespace FBS.Domain.Flight.Exceptions;

public class FlightNotFoundException(FlightId flightId) 
    : DomainException($"Flight with ID {flightId} was not found")
{
    public FlightId FlightId { get; } = flightId;
}
