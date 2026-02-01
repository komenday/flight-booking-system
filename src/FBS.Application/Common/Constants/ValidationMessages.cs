namespace FBS.Application.Common.Constants;

public static class ValidationMessages
{
    public const string EmailRequired = "Email is required";
    public const string EmailInvalid = "Invalid email format";

    public const string PhoneRequired = "Phone number is required";
    public const string PhoneInvalid = "Invalid phone number format. Expected E.164 format (e.g., +380501234567)";

    public const string SeatNumberRequired = "Seat number is required";
    public const string SeatNumberInvalid = "Invalid seat number format. Expected format: 1A, 12F, etc.";

    public const string FlightNumberRequired = "Flight number is required";
    public const string FlightNumberInvalid = "Invalid flight number format. Expected format: UA123, BA4567";

    public const string AirportCodeInvalid = "Airport code must be 3 characters (IATA code)";

    public const string NameRequired = "Name is required";
    public const string NameInvalid = "Name contains invalid characters";
    public const string NameTooLong = "Name must not exceed {0} characters";
}
