using FBS.Domain.Common.Exceptions;

namespace FBS.Domain.Flight.Exceptions;

public class FlightAlreadyDepartedException(FlightId flightId, DateTime departureTime)
    : DomainException($"Flight {flightId} has already departed at {departureTime:yyyy-MM-dd HH:mm}")
{
    public FlightId FlightId { get; } = flightId;
    public DateTime DepartureTime { get; } = departureTime;
}
