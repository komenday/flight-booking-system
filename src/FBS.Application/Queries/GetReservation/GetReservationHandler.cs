using FBS.Application.Common.Result;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using MediatR;

namespace FBS.Application.Queries.GetReservation;

public class GetReservationHandler : IRequestHandler<GetReservationQuery, Result<ReservationDetailsDto>>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IFlightRepository _flightRepository;

    public GetReservationHandler(
        IReservationRepository reservationRepository,
        IFlightRepository flightRepository)
    {
        _reservationRepository = reservationRepository;
        _flightRepository = flightRepository;
    }

    public async Task<Result<ReservationDetailsDto>> Handle(GetReservationQuery request, CancellationToken cancellationToken)
    {
        var reservationId = ReservationId.From(request.ReservationId);

        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation is null)
        {
            return Result.NotFound<ReservationDetailsDto>($"Reservation with ID {request.ReservationId} was not found");
        }

        var flight = await _flightRepository.GetByIdAsync(reservation.FlightId, cancellationToken);

        if (flight is null)
        {
            return Result.NotFound<ReservationDetailsDto>($"Flight with ID {reservation.FlightId} was not found");
        }

        var reservationDetails = new ReservationDetailsDto(
            reservation.Id.Value,
            flight.Id.Value,
            flight.Number.Value,
            flight.Departure.IataCode,
            flight.Arrival.IataCode,
            flight.DepartureTime,
            reservation.SeatNumber.Value,
            reservation.Passenger.FirstName,
            reservation.Passenger.LastName,
            reservation.Passenger.Email.Value,
            reservation.Passenger.Phone.Value,
            reservation.Status.ToString(),
            reservation.CreatedAt,
            reservation.ExpiresAt,
            reservation.ConfirmedAt
        );

        return Result.Success(reservationDetails);
    }
}
