using FBS.Domain.Reservation;

namespace FBS.Application.Exceptions;

public class ReservationNotFoundException(ReservationId reservationId)
    : NotFoundException($"Reservation with ID {reservationId} was not found")
{
    public ReservationId ReservationId { get; } = reservationId;
}
