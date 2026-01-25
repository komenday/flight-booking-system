using FBS.Domain.Common.Exceptions;

namespace FBS.Domain.Reservation.Exceptions;

public class InvalidReservationStateException(
    ReservationId reservationId,
    ReservationStatus currentStatus,
    string attemptedAction)
    : DomainException($"Cannot {attemptedAction} reservation {reservationId} in status {currentStatus}")
{
    public ReservationId ReservationId { get; } = reservationId;

    public ReservationStatus CurrentStatus { get; } = currentStatus;

    public string AttemptedAction { get; } = attemptedAction;
}
