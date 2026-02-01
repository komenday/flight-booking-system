using FBS.Application.Common.Result;
using MediatR;

namespace FBS.Application.Queries.GetReservation;

public record GetReservationQuery(Guid ReservationId)
    : IRequest<Result<ReservationDetailsDto>>;