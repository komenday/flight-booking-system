using MediatR;

namespace FBS.Application.Commands.CreateReservation;

public record CreateReservationCommand(
    Guid FlightId,
    string SeatNumber,
    string FirstName,
    string LastName,
    string Email,
    string Phone
) : IRequest<CreateReservationResponse>;