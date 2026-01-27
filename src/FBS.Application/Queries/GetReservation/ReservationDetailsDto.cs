namespace FBS.Application.Queries.GetReservation;

public record ReservationDetailsDto(
    Guid ReservationId,
    Guid FlightId,
    string FlightNumber,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime DepartureTime,
    string SeatNumber,
    string PassengerFirstName,
    string PassengerLastName,
    string PassengerEmail,
    string PassengerPhone,
    string Status,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    DateTime? ConfirmedAt
);