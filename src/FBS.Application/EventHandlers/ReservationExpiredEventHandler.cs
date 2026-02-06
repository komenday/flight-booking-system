using FBS.Domain.Repositories;
using FBS.Domain.Reservation.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.EventHandlers;

public class ReservationExpiredEventHandler : INotificationHandler<ReservationExpiredEvent>
{
    private readonly IFlightRepository _flightRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReservationExpiredEventHandler> _logger;

    public ReservationExpiredEventHandler(
        IFlightRepository flightRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReservationExpiredEventHandler> logger)
    {
        _flightRepository = flightRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ReservationExpiredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ReservationExpiredEvent for reservation {ReservationId}", notification.ReservationId.Value);

        try
        {
            var flight = await _flightRepository.GetByIdAsync(notification.FlightId, cancellationToken);

            if (flight is null)
            {
                _logger.LogWarning("Flight {FlightId} not found when handling expired reservation {ReservationId}", notification.FlightId.Value, notification.ReservationId.Value);
                return;
            }

            flight.ReleaseSeat(notification.SeatNumber);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Seat {SeatNumber} released for flight {FlightId} due to expired reservation {ReservationId}", notification.SeatNumber.Value, notification.FlightId.Value, notification.ReservationId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ReservationExpiredEvent for reservation {ReservationId}", notification.ReservationId.Value);
            throw;
        }
    }
}
