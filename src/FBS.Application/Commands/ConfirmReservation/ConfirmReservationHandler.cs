using FBS.Application.Common.Result;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using FBS.Domain.Reservation.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.ConfirmReservation;

public class ConfirmReservationHandler : IRequestHandler<ConfirmReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<ConfirmReservationHandler> _logger;

    public ConfirmReservationHandler(
        IReservationRepository reservationRepository,
        ILogger<ConfirmReservationHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Confirming reservation {ReservationId}", request.ReservationId);

        var reservationId = ReservationId.From(request.ReservationId);
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure($"Reservation with ID {request.ReservationId} was not found");
        }

        try
        {
            reservation.Confirm();
        }
        catch (InvalidReservationStateException ex)
        {
            return Result.Failure(ex.Message, ex);
        }
        catch (ReservationExpiredException ex)
        {
            return Result.Failure(ex.Message, ex);
        }

        _logger.LogInformation("Reservation {ReservationId} confirmed", reservationId);

        return Result.Success();
    }
}
