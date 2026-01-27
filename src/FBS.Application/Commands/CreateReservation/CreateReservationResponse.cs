namespace FBS.Application.Commands.CreateReservation;

public record CreateReservationResponse(
    Guid ReservationId,
    Guid FlightId,
    string FlightNumber,
    string SeatNumber,
    string PassengerName,
    string Status,
    DateTime CreatedAt,
    DateTime ExpiresAt
);