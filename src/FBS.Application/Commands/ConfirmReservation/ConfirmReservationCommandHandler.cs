using FBS.Application.Common.Result;
using FBS.Application.Extensions;
using FBS.Domain.Common.Exceptions;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.ConfirmReservation;

public class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<ConfirmReservationCommandHandler> _logger;

    public ConfirmReservationCommandHandler(
        IReservationRepository reservationRepository,
        ILogger<ConfirmReservationCommandHandler> logger)
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
            return Result.NotFound($"Reservation with ID {request.ReservationId} was not found");
        }

        try
        {
            reservation.Confirm();
        }
        catch (BusinessRuleValidationException ex)
        {
            _logger.LogWarning(ex, "Business rule validation failed: {RuleName} ({ErrorType})", ex.BrokenRule.GetType().Name, ex.RuleErrorType);
            var errorType = ex.RuleErrorType.ToErrorType();
            return Result.Failure(ex.Message, errorType, ex);
        }

        _logger.LogInformation("Reservation {ReservationId} confirmed", reservationId);

        return Result.Success();
    }
}
