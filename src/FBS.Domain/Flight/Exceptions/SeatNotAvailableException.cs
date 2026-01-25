using FBS.Domain.Common.Exceptions;

namespace FBS.Domain.Flight.Exceptions;

public class SeatNotAvailableException(FlightNumber flightNumber, SeatNumber seatNumber)
    : DomainException($"Seat {seatNumber} on flight {flightNumber} is not available")
{
    public FlightNumber FlightNumber { get; } = flightNumber;

    public SeatNumber SeatNumber { get; } = seatNumber;
}