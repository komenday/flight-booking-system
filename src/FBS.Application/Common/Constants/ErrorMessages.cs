namespace FBS.Application.Common.Constants;

public static class ErrorMessages
{
    public static string FlightNotFound(Guid id)
        => $"Flight with ID {id} was not found";

    public static string SeatNotAvailable(string flightNumber, string seatNumber)
        => $"Seat {seatNumber} on flight {flightNumber} is not available";

    public static string ReservationExpired(Guid id)
        => $"Reservation {id} has expired";
}
