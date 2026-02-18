using FBS.Application.Common.Result;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.ExpireReservation;

public class ExpireReservationCommandHandler(
    IFlightRepository flightRepository,
    IReservationRepository reservationRepository,
    ILogger<ExpireReservationCommandHandler> logger) : IRequestHandler<ExpireReservationCommand, Result>
{
    private readonly IFlightRepository _flightRepository = flightRepository;
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly ILogger<ExpireReservationCommandHandler> _logger = logger;

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

        var flight = await _flightRepository.GetByIdAsync(reservation.FlightId, cancellationToken);

        if (flight is null)
        {
            _logger.LogError("Flight {FlightId} not found for reservation {ReservationId}", reservation.FlightId.Value, request.ReservationId);
            return Result.NotFound($"Flight {reservation.FlightId.Value} not found");
        }

        reservation.Expire();
        flight.ReleaseSeat(reservation.SeatNumber);

        _logger.LogInformation("Reservation {ReservationId} expired and seat {SeatNumber} released on flight {FlightId}",
            reservationId.Value, reservation.SeatNumber.Value, reservation.FlightId.Value);

        return Result.Success();
    }
}
