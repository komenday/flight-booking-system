using FBS.Domain.Common.Exceptions;

namespace FBS.Domain.Flight.Exceptions;

public class SeatNotFoundException(FlightNumber flightNumber, SeatNumber seatNumber)
    : DomainException($"Seat {seatNumber} not found on flight {flightNumber}")
{
    public FlightNumber FlightNumber { get; } = flightNumber;

    public SeatNumber SeatNumber { get; } = seatNumber;
}
