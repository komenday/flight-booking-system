using FBS.Application.Common.Result;
using FBS.Application.Extensions;
using FBS.Domain.Common.Exceptions;
using FBS.Domain.Flight;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.CreateReservation;

public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Result<CreateReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;

    private readonly IFlightRepository _flightRepository;

    private readonly ILogger<CreateReservationCommandHandler> _logger;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IFlightRepository flightRepository,
        ILogger<CreateReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository;
        _flightRepository = flightRepository;
        _logger = logger;
    }

    public async Task<Result<CreateReservationResponse>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating reservation for flight {FlightId}, seat {SeatNumber}", request.FlightId, request.SeatNumber);

        var flightId = FlightId.From(request.FlightId);
        var flight = await _flightRepository.GetByIdAsync(flightId, cancellationToken);

        if (flight is null)
        {
            return Result.NotFound<CreateReservationResponse>($"Flight with ID {request.FlightId} was not found");
        }

        var seatNumber = SeatNumber.From(request.SeatNumber);
        var passengerInfo = PassengerInfo.Create(
            request.Passenger.FirstName,
            request.Passenger.LastName,
            request.Passenger.Email,
            request.Passenger.Phone
        );

        try
        {
            flight.ReserveSeat(seatNumber);
        }
        catch (BusinessRuleValidationException ex)
        {
            _logger.LogWarning(ex, "Business rule validation failed: {RuleName} ({ErrorType})", ex.BrokenRule.GetType().Name, ex.RuleErrorType);
            var errorType = ex.RuleErrorType.ToErrorType();
            return Result.Failure<CreateReservationResponse>(ex.Message, errorType, ex);
        }

        var reservation = Reservation.Create(flight.Id, seatNumber, passengerInfo);
        await _reservationRepository.AddAsync(reservation, cancellationToken);

        _logger.LogInformation("Reservation {ReservationId} created successfully", reservation.Id);

        var response = new CreateReservationResponse(
            reservation.Id.Value,
            flight.Id.Value,
            flight.Number.Value,
            reservation.SeatNumber.Value,
            reservation.Passenger.FullName,
            reservation.Status.ToString(),
            reservation.CreatedAt,
            reservation.ExpiresAt
        );

        return Result.Success(response);
    }
}
