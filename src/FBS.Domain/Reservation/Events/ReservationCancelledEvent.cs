using FBS.Domain.Common.Base;
using FBS.Domain.Flight;

namespace FBS.Domain.Reservation.Events;

public record ReservationCancelledEvent : DomainEventBase
{
    public ReservationCancelledEvent(
        ReservationId reservationId,
        FlightId flightId,
        SeatNumber seatNumber)
    {
        ReservationId = reservationId;
        FlightId = flightId;
        SeatNumber = seatNumber;
    }

    public ReservationId ReservationId { get; init; }

    public FlightId FlightId { get; init; }

    public SeatNumber SeatNumber { get; init; }
}
