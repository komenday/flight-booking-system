using FBS.Application.Common.Result;
using FBS.Application.Exceptions;
using FBS.Domain.Flight.Exceptions;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using FBS.Domain.Reservation.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.CancelReservation;

public class CancelReservationHandler : IRequestHandler<CancelReservationCommand, Result>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly ILogger<CancelReservationHandler> _logger;

    public CancelReservationHandler(
        IReservationRepository reservationRepository,
        IFlightRepository flightRepository,
        ILogger<CancelReservationHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _flightRepository = flightRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling reservation {ReservationId}", request.ReservationId);

        var reservationId = ReservationId.From(request.ReservationId);
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.Failure($"Reservation with ID {request.ReservationId} was not found");
        }

        var flight = await _flightRepository.GetByIdAsync(reservation.FlightId, cancellationToken) ?? throw new FlightNotFoundException(reservation.FlightId);

        if (flight is null)
        {
            return Result.Failure($"Flight with ID {reservation.FlightId} was not found");
        }

        try
        {
            reservation.Cancel();
            flight.ReleaseSeat(reservation.SeatNumber);
        }
        catch (InvalidReservationStateException ex)
        {
            return Result.Failure(ex.Message, exception: ex);
        }
        catch (SeatNotFoundException ex)
        {
            _logger.LogWarning(ex, "Seat {SeatNumber} not found when cancelling reservation {ReservationId}", reservation.SeatNumber, reservationId);
        }

        _logger.LogInformation("Reservation {ReservationId} cancelled", reservationId);

        return Result.Success();
    }
}
