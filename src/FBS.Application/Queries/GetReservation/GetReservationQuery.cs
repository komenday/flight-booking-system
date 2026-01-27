using MediatR;

namespace FBS.Application.Queries.GetReservation;

public record GetReservationQuery(Guid ReservationId)
    : IRequest<ReservationDetailsDto?>;