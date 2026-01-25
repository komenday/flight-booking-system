using FBS.Domain.Common.Exceptions;

namespace FBS.Domain.Reservation.Exceptions;

public class ReservationExpiredException(ReservationId reservationId, DateTime expiresAt)
    : DomainException($"Reservation {reservationId} expired at {expiresAt:yyyy-MM-dd HH:mm:ss} UTC")
{
    public ReservationId ReservationId { get; } = reservationId;

    public DateTime ExpiresAt { get; } = expiresAt;
}
