using FBS.Domain.Common.Base;
using FBS.Domain.Flight;

namespace FBS.Domain.Reservation.Events;

public record ReservationCreatedEvent : ExternalDomainEventBase
{
    public ReservationCreatedEvent(
        ReservationId reservationId,
        FlightId flightId,
        FlightNumber flightNumber,
        SeatNumber seatNumber,
        PassengerInfo passenger,
        DateTime expiresAt)
    {
        ReservationId = reservationId;
        FlightId = flightId;
        FlightNumber = flightNumber;
        SeatNumber = seatNumber;
        Passenger = passenger;
        ExpiresAt = expiresAt;
    }

    public ReservationId ReservationId { get; init; }

    public FlightId FlightId { get; init; }

    public FlightNumber FlightNumber { get; init; }

    public SeatNumber SeatNumber { get; init; }

    public PassengerInfo Passenger { get; init; }

    public DateTime ExpiresAt { get; init; }
}
