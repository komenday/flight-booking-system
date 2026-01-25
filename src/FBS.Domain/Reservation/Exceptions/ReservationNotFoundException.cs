using FBS.Domain.Common.Exceptions;

namespace FBS.Domain.Reservation.Exceptions;

public class ReservationNotFoundException(ReservationId reservationId)
    : DomainException($"Reservation with ID {reservationId} was not found")
{
    public ReservationId ReservationId { get; } = reservationId;
}
