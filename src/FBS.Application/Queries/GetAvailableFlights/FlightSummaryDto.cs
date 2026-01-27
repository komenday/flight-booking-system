namespace FBS.Application.Queries.GetAvailableFlights;

public record FlightSummaryDto(
    Guid FlightId,
    string FlightNumber,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime DepartureTime,
    int AvailableSeats
);