namespace FBS.Application.Commands.CreateReservation;

public record PassengerInfoDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone
);
