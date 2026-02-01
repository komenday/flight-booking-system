using FBS.Application.Common.Result;
using MediatR;

namespace FBS.Application.Commands.CancelReservation;

public record CancelReservationCommand(Guid ReservationId) : IRequest<Result>;