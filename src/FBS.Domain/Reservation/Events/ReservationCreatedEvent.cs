using FBS.Domain.Common.Base;
using FBS.Domain.Flight;

namespace FBS.Domain.Reservation.Events;

public record ReservationCreatedEvent : DomainEventBase
{
    public ReservationCreatedEvent(
        ReservationId reservationId,
        FlightId flightId,
        SeatNumber seatNumber,
        DateTime expiresAt)
    {
        ReservationId = reservationId;
        FlightId = flightId;
        SeatNumber = seatNumber;
        ExpiresAt = expiresAt;
    }

    public ReservationId ReservationId { get; init; }

    public FlightId FlightId { get; init; }

    public SeatNumber SeatNumber { get; init; }

    public DateTime ExpiresAt { get; init; }
}
