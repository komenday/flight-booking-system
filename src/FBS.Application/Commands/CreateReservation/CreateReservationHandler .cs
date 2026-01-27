using FBS.Application.Exceptions;
using FBS.Domain.Flight;
using FBS.Domain.Flight.Exceptions;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.CreateReservation;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, CreateReservationResponse>
{
    private readonly IReservationRepository _reservationRepository;

    private readonly IFlightRepository _flightRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<CreateReservationHandler> _logger;

    public CreateReservationHandler(
        IReservationRepository reservationRepository,
        IFlightRepository flightRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateReservationHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _flightRepository = flightRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateReservationResponse> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating reservation for flight {FlightId}, seat {SeatNumber}", request.FlightId, request.SeatNumber);

        var flightId = FlightId.From(request.FlightId);
        var flight = await _flightRepository.GetByIdAsync(flightId, cancellationToken) ?? throw new FlightNotFoundException(flightId);

        var seatNumber = SeatNumber.From(request.SeatNumber);
        var passengerInfo = PassengerInfo.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone
        );

        flight.ReserveSeat(seatNumber);

        var reservation = Reservation.Create(
            flight.Id,
            seatNumber,
            passengerInfo
        );

        await _reservationRepository.AddAsync(reservation, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogWarning("Concurrency conflict when reserving seat {SeatNumber} on flight {FlightId}", seatNumber, flightId);
            throw new SeatNotAvailableException(flight.Number, seatNumber);
        }

        _logger.LogInformation("Reservation {ReservationId} created successfully", reservation.Id);

        return new CreateReservationResponse(
            reservation.Id.Value,
            flight.Id.Value,
            flight.Number.Value,
            reservation.SeatNumber.Value,
            reservation.Passenger.FullName,
            reservation.Status.ToString(),
            reservation.CreatedAt,
            reservation.ExpiresAt
        );
    }
}
