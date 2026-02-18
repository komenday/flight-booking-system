namespace FBS.Infrastructure.Events.DTOs;
public abstract record ReservationEventDto
{
    public Guid EventId { get; init; }
    public DateTime OccurredAt { get; init; }
    public Guid ReservationId { get; init; }
    public Guid FlightId { get; init; }
    public string FlightNumber { get; init; } = string.Empty;
    public string SeatNumber { get; init; } = string.Empty;
    public string PassengerFirstName { get; init; } = string.Empty;
    public string PassengerLastName { get; init; } = string.Empty;
    public string PassengerEmail { get; init; } = string.Empty;
}

public record ReservationCreatedEventDto : ReservationEventDto
{
    public DateTime ExpiresAt { get; init; }
}

public record ReservationConfirmedEventDto : ReservationEventDto
{
}

public record ReservationCancelledEventDto : ReservationEventDto
{
}

public record ReservationExpiredEventDto : ReservationEventDto
{
}