namespace FBS.API.DTOs;

public record CreateReservationRequest(
    Guid FlightId,
    string SeatNumber,
    string FirstName,
    string LastName,
    string Email,
    string Phone
);