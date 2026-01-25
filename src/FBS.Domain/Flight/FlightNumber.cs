using System.Text.RegularExpressions;

namespace FBS.Domain.Flight;

public partial record FlightNumber
{
    public string Value { get; }

    private FlightNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Flight number is required");

        var regex = ValidFlightNumberRegex();
        if (!regex.IsMatch(value))
            throw new ArgumentException($"Invalid flight number: {value}");

        Value = value.ToUpper();
    }

    public static FlightNumber From(string value) => new(value);

    public string Airline => Value[..2];
    public string Number => Value[2..];

    [GeneratedRegex(@"^[A-Z]{2}\d{3,4}$", RegexOptions.IgnoreCase)]
    private static partial Regex ValidFlightNumberRegex();
}
