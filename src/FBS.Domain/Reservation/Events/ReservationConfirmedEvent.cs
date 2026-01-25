using FBS.Domain.Common.Base;

namespace FBS.Domain.Reservation.Events;

public record ReservationConfirmedEvent : DomainEventBase
{
    public ReservationConfirmedEvent(ReservationId reservationId)
    {
        ReservationId = reservationId;
        ConfirmedAt = DateTime.UtcNow;
    }

    public ReservationId ReservationId { get; init; }

    public DateTime ConfirmedAt { get; init; }

}
