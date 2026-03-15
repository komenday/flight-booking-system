using FBS.Function.Notifications.Interfaces;
using FBS.Function.Notifications.Templates;
using FBS.Infrastructure.Events.DTOs;
using Microsoft.Extensions.Logging;

namespace FBS.Function.Notifications.Notification;

public class NotificationService(
    IEmailService emailService,
    ILogger<NotificationService> logger) : INotificationService
{
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<NotificationService> _logger = logger;

    public async Task HandleReservationCreatedAsync(ReservationCreatedEventDto @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing ReservationCreated notification for {ReservationId}", @event.ReservationId);

        var passengerName = $"{@event.PassengerFirstName} {@event.PassengerLastName}";
        var emailContent = EmailTemplates.ReservationCreated(
            passengerName: passengerName,
            flightNumber: @event.FlightNumber,
            seatNumber: @event.SeatNumber,
            expiresAt: @event.ExpiresAt
        );

        await _emailService.SendAsync(
            to: @event.PassengerEmail,
            subject: "✈️ Reservation Created - Confirm Within 10 Minutes",
            htmlContent: emailContent,
            cancellationToken
        );

        _logger.LogInformation("Sent ReservationCreated notification to {Email}", @event.PassengerEmail);
    }

    public async Task HandleReservationConfirmedAsync(ReservationConfirmedEventDto @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing ReservationConfirmed notification for {ReservationId}", @event.ReservationId);

        var passengerName = $"{@event.PassengerFirstName} {@event.PassengerLastName}";
        var emailContent = EmailTemplates.ReservationConfirmed(
            passengerName: passengerName,
            flightNumber: @event.FlightNumber,
            seatNumber: @event.SeatNumber
        );

        await _emailService.SendAsync(
            to: @event.PassengerEmail,
            subject: "✅ Reservation Confirmed - You're All Set!",
            htmlContent: emailContent,
            cancellationToken
        );

        _logger.LogInformation("Sent ReservationConfirmed notification to {Email}", @event.PassengerEmail);
    }

    public async Task HandleReservationCancelledAsync(ReservationCancelledEventDto @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing ReservationCancelled notification for {ReservationId}", @event.ReservationId);

        var passengerName = $"{@event.PassengerFirstName} {@event.PassengerLastName}";
        var emailContent = EmailTemplates.ReservationCancelled(
            passengerName: passengerName,
            flightNumber: @event.FlightNumber,
            seatNumber: @event.SeatNumber
        );

        await _emailService.SendAsync(
            to: @event.PassengerEmail,
            subject: "❌ Reservation Cancelled",
            htmlContent: emailContent,
            cancellationToken
        );

        _logger.LogInformation("Sent ReservationCancelled notification to {Email}", @event.PassengerEmail);
    }

    public async Task HandleReservationExpiredAsync(ReservationExpiredEventDto @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing ReservationExpired notification for {ReservationId}", @event.ReservationId);

        var passengerName = $"{@event.PassengerFirstName} {@event.PassengerLastName}";
        var emailContent = EmailTemplates.ReservationExpired(
            passengerName: passengerName,
            flightNumber: @event.FlightNumber,
            seatNumber: @event.SeatNumber
        );

        await _emailService.SendAsync(
            to: @event.PassengerEmail,
            subject: "⏰ Reservation Expired",
            htmlContent: emailContent,
            cancellationToken
        );

        _logger.LogInformation("Sent ReservationExpired notification to {Email}", @event.PassengerEmail);
    }
}