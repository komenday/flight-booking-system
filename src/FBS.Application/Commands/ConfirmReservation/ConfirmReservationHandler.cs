using FBS.Application.Exceptions;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.ConfirmReservation;

public class ConfirmReservationHandler : IRequestHandler<ConfirmReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConfirmReservationHandler> _logger;

    public ConfirmReservationHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ILogger<ConfirmReservationHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ConfirmReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Confirming reservation {ReservationId}", request.ReservationId);

        var reservationId = ReservationId.From(request.ReservationId);
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken)
            ?? throw new ReservationNotFoundException(reservationId);

        reservation.Confirm();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Reservation {ReservationId} confirmed successfully", reservationId);
    }
}
