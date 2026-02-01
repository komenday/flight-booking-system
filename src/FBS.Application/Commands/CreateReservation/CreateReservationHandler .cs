using FBS.Application.Common.Result;
using FBS.Domain.Flight;
using FBS.Domain.Flight.Exceptions;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Commands.CreateReservation;

public class CreateReservationHandler : IRequestHandler<CreateReservationCommand, Result<CreateReservationResponse>>
{
    private readonly IReservationRepository _reservationRepository;

    private readonly IFlightRepository _flightRepository;

    private readonly ILogger<CreateReservationHandler> _logger;

    public CreateReservationHandler(
        IReservationRepository reservationRepository,
        IFlightRepository flightRepository,
        ILogger<CreateReservationHandler> logger)
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
            return Result.Failure<CreateReservationResponse>($"Flight with ID {request.FlightId} was not found");
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
        catch (SeatNotFoundException ex)
        {
            return Result.Failure<CreateReservationResponse>($"Seat {seatNumber.Value} not found on flight {flight.Number.Value}", ex);
        }
        catch (SeatNotAvailableException ex)
        {
            return Result.Failure<CreateReservationResponse>($"Seat {seatNumber.Value} on flight {flight.Number.Value} is not available", ex);
        }
        catch (FlightAlreadyDepartedException ex)
        {
            return Result.Failure<CreateReservationResponse>($"Flight {flight.Number.Value} has already departed", ex);
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
