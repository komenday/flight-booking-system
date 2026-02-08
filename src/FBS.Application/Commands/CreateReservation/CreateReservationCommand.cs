using FBS.Application.Common.Result;
using MediatR;

namespace FBS.Application.Commands.CreateReservation;

public record CreateReservationCommand(
    Guid FlightId,
    string SeatNumber,
    PassengerInfoDto Passenger
) : IRequest<Result<CreateReservationResponse>>;

public record PassengerInfoDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone
);