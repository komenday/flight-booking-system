using FBS.Application.Common.Result;
using FBS.Application.Exceptions;
using FBS.Application.Extensions;
using FBS.Domain.Common.Exceptions;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.CancelReservation;

public class CancelReservationCommandHandler(
    IReservationRepository reservationRepository,
    IFlightRepository flightRepository,
    ILogger<CancelReservationCommandHandler> logger) : IRequestHandler<CancelReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly IFlightRepository _flightRepository = flightRepository;
    private readonly ILogger<CancelReservationCommandHandler> _logger = logger;

    public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling reservation {ReservationId}", request.ReservationId);

        var reservationId = ReservationId.From(request.ReservationId);
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.NotFound($"Reservation with ID {request.ReservationId} was not found");
        }

        var flight = await _flightRepository.GetByIdAsync(reservation.FlightId, cancellationToken) ?? throw new FlightNotFoundException(reservation.FlightId);

        if (flight is null)
        {
            return Result.NotFound($"Flight with ID {reservation.FlightId} was not found");
        }

        try
        {
            reservation.Cancel();
            flight.ReleaseSeat(reservation.SeatNumber);
        }
        catch (BusinessRuleValidationException ex)
        {
            _logger.LogWarning(ex, "Business rule validation failed: {RuleName} ({ErrorType})", ex.BrokenRule.GetType().Name, ex.RuleErrorType);
            var errorType = ex.RuleErrorType.ToErrorType();
            return Result.Failure(ex.Message, errorType, ex);
        }

        _logger.LogInformation("Reservation {ReservationId} cancelled", reservationId);

        return Result.Success();
    }
}
