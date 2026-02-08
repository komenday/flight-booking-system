namespace FBS.API.DTOs;

public record SearchFlightsRequest(
    string DepartureAirport,
    string ArrivalAirport,
    DateTime Date
);
