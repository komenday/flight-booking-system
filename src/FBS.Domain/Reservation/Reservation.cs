using FBS.Domain.Common.Base;
using FBS.Domain.Flight;
using FBS.Domain.Reservation.Events;
using FBS.Domain.Reservation.Exceptions;

namespace FBS.Domain.Reservation;

public class Reservation : AggregateRoot<ReservationId>
{
    public FlightId FlightId { get; private set; } = null!;

    public SeatNumber SeatNumber { get; private set; } = null!;

    public PassengerInfo Passenger { get; private set; } = null!;

    public ReservationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public DateTime? ConfirmedAt { get; private set; }

    public bool IsActive
        => Status is ReservationStatus.Pending or ReservationStatus.Confirmed;

    public bool CanBeConfirmed
        => Status is ReservationStatus.Pending && DateTime.UtcNow <= ExpiresAt;

    public bool HasExpired
        => Status is ReservationStatus.Pending && DateTime.UtcNow > ExpiresAt;

    private Reservation() { }

    public static Reservation Create(
        FlightId flightId,
        SeatNumber seatNumber,
        PassengerInfo passenger)
    {
        ArgumentNullException.ThrowIfNull(flightId);
        ArgumentNullException.ThrowIfNull(seatNumber);
        ArgumentNullException.ThrowIfNull(passenger);

        var reservation = new Reservation
        {
            Id = ReservationId.New(),
            FlightId = flightId,
            SeatNumber = seatNumber,
            Passenger = passenger,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        reservation.AddDomainEvent(new ReservationCreatedEvent(
            reservation.Id,
            reservation.FlightId,
            reservation.SeatNumber,
            reservation.ExpiresAt
        ));

        return reservation;
    }

    public void Confirm()
    {
        if (Status is not ReservationStatus.Pending)
            throw new InvalidReservationStateException(Id, Status, nameof(Confirm));

        if (DateTime.UtcNow > ExpiresAt)
            throw new ReservationExpiredException(Id, ExpiresAt);

        Status = ReservationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationConfirmedEvent(Id));
    }

    public void Cancel()
    {
        if (Status is ReservationStatus.Expired or ReservationStatus.Cancelled)
            throw new InvalidReservationStateException(Id, Status, nameof(Cancel));

        Status = ReservationStatus.Cancelled;

        AddDomainEvent(new ReservationCancelledEvent(Id, FlightId, SeatNumber));
    }

    public void Expire()
    {
        if (Status is not ReservationStatus.Pending)
            return;

        Status = ReservationStatus.Expired;
        AddDomainEvent(new ReservationExpiredEvent(Id, FlightId, SeatNumber));
    }
}
