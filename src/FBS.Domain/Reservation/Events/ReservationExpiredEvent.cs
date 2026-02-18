using FBS.Domain.Common.Base;
using FBS.Domain.Flight;

namespace FBS.Domain.Reservation.Events;

public record ReservationExpiredEvent : ExternalDomainEventBase
{
    public ReservationExpiredEvent(
        ReservationId reservationId,
        FlightId flightId,
        FlightNumber flightNumber,
        SeatNumber seatNumber,
        PassengerInfo passenger)
    {
        ReservationId = reservationId;
        FlightId = flightId;
        FlightNumber = flightNumber;
        SeatNumber = seatNumber;
        Passenger = passenger;
    }

    public ReservationId ReservationId { get; init; }

    public FlightId FlightId { get; init; }

    public FlightNumber FlightNumber { get; init; }

    public SeatNumber SeatNumber { get; init; }

    public PassengerInfo Passenger { get; init; }
}