using MediatR;

namespace FBS.Application.Commands.ConfirmReservation;

public record ConfirmReservationCommand(Guid ReservationId) : IRequest;