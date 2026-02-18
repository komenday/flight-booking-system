namespace FBS.API.DTOs;

public record CreateReservationRequest(
    string FlightNumber,
    string SeatNumber,
    string FirstName,
    string LastName,
    string Email,
    string Phone
);