using FBS.Application.Common.Result;
using MediatR;

namespace FBS.Application.Commands.ExpireReservation;

public record ExpireReservationCommand(Guid ReservationId) : IRequest<Result>;