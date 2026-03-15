using FBS.Infrastructure.Events.DTOs;

namespace FBS.Function.Notifications.Interfaces;

public interface INotificationService
{
    Task HandleReservationCreatedAsync(ReservationCreatedEventDto @event, CancellationToken cancellationToken);

    Task HandleReservationConfirmedAsync(ReservationConfirmedEventDto @event, CancellationToken cancellationToken);

    Task HandleReservationCancelledAsync(ReservationCancelledEventDto @event, CancellationToken cancellationToken);

    Task HandleReservationExpiredAsync(ReservationExpiredEventDto @event, CancellationToken cancellationToken);
}