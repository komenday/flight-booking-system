using FBS.Application.Common.Result;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.ExpireReservation;

public class ExpireReservationCommandHandler : IRequestHandler<ExpireReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<ExpireReservationCommandHandler> _logger;

    public ExpireReservationCommandHandler(
        IReservationRepository reservationRepository,
        ILogger<ExpireReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(ExpireReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to expire reservation {ReservationId}", request.ReservationId);

        var reservationId = ReservationId.From(request.ReservationId);
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation is null)
        {
            _logger.LogWarning("Reservation {ReservationId} not found during expiration attempt", request.ReservationId);
            return Result.Success();
        }

        reservation.Expire();

        _logger.LogInformation("Reservation {ReservationId} successfully expired", reservationId);

        return Result.Success();
    }
}
