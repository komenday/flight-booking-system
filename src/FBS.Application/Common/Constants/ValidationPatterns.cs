namespace FBS.Application.Common.Constants;

public static class ValidationPatterns
{
    public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public const string Phone = @"^\+?[1-9]\d{1,14}$";

    public const string SeatNumber = @"^[1-9][0-9]?[A-F]$";

    public const string FlightNumber = @"^[A-Z]{2}\d{3,4}$";

    public const string AirportCode = @"^[A-Z]{3}$";

    public const string Name = @"^[a-zA-Z\s\-']+$";
}