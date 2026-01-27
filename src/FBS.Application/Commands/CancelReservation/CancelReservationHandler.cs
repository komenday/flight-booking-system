using FBS.Application.Exceptions;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.CancelReservation;

public class CancelReservationHandler : IRequestHandler<CancelReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IFlightRepository _flightRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelReservationHandler> _logger;

    public CancelReservationHandler(
        IReservationRepository reservationRepository,
        IFlightRepository flightRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelReservationHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _flightRepository = flightRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(CancelReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling reservation {ReservationId}", request.ReservationId);

        var reservationId = ReservationId.From(request.ReservationId);
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken) ?? throw new ReservationNotFoundException(reservationId);

        var flight = await _flightRepository.GetByIdAsync(reservation.FlightId, cancellationToken) ?? throw new FlightNotFoundException(reservation.FlightId);

        reservation.Cancel();
        flight.ReleaseSeat(reservation.SeatNumber);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogWarning("Concurrency conflict when cancelling reservation {ReservationId}", reservationId);
            throw;
        }

        _logger.LogInformation("Reservation {ReservationId} cancelled successfully", reservationId);
    }
}
