using FBS.Domain.Common.Base;
using FBS.Domain.Common.Rules;
using FBS.Domain.Flight;
using FBS.Domain.Reservation.Events;

namespace FBS.Domain.Reservation;

public class Reservation : AggregateRoot<ReservationId>
{
    public FlightId FlightId { get; private set; } = null!;

    public FlightNumber FlightNumber { get; private set; } = null!;

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
        FlightNumber flightNumber,
        SeatNumber seatNumber,
        PassengerInfo passenger)
    {
        ArgumentNullException.ThrowIfNull(flightId);
        ArgumentNullException.ThrowIfNull(flightNumber);
        ArgumentNullException.ThrowIfNull(seatNumber);
        ArgumentNullException.ThrowIfNull(passenger);

        var reservation = new Reservation
        {
            Id = ReservationId.New(),
            FlightId = flightId,
            FlightNumber = flightNumber,
            SeatNumber = seatNumber,
            Passenger = passenger,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };

        reservation.AddDomainEvent(new ReservationCreatedEvent(
            reservation.Id,
            reservation.FlightId,
            reservation.FlightNumber,
            reservation.SeatNumber,
            reservation.Passenger,
            reservation.ExpiresAt
        ));

        return reservation;
    }

    public void Confirm()
    {
        this.CheckRules(
            new ReservationMustBePendingToConfirmRule(Id, Status),
            new ReservationMustNotBeExpiredToConfirmRule(Id, ExpiresAt)
        );

        Status = ReservationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        AddDomainEvent(new ReservationConfirmedEvent(
            Id,
            FlightId,
            FlightNumber,
            SeatNumber,
            Passenger
        ));
    }

    public void Cancel()
    {
        this.CheckRule(new CannotCancelExpiredOrCancelledReservationRule(Id, Status));

        Status = ReservationStatus.Cancelled;

        AddDomainEvent(new ReservationCancelledEvent(
            Id,
            FlightId,
            FlightNumber,
            SeatNumber,
            Passenger
        ));
    }

    public void Expire()
    {
        var rule = new OnlyPendingReservationsCanExpireRule(Status);
        if (rule.IsBroken())
            return;

        Status = ReservationStatus.Expired;

        AddDomainEvent(new ReservationExpiredEvent(
            Id,
            FlightId,
            FlightNumber,
            SeatNumber,
            Passenger
        ));
    }
}
