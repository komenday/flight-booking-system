namespace FBS.Application.Queries.GetFlightByNumber;

public record FlightDetailsDto(
    Guid FlightId,
    string FlightNumber,
    string DepartureAirport,
    string ArrivalAirport,
    DateTime DepartureTime,
    int TotalSeats,
    int AvailableSeats,
    List<SeatDto> Seats
);

public record SeatDto(
    string SeatNumber,
    bool IsAvailable
);
