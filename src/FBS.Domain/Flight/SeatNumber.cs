using System.Text.RegularExpressions;

namespace FBS.Domain.Flight;

public partial record SeatNumber
{
    public string Value { get; }

    private SeatNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Seat number cannot be empty");

        if (!IsValidFormat(value))
            throw new ArgumentException($"Invalid seat number format: {value}");

        Value = value.ToUpper();
    }

    public static SeatNumber From(string value) => new(value);

    private static bool IsValidFormat(string value)
    {
        var regex = ValidSeatNumberRegex();
        return regex.IsMatch(value);
    }

    public int Row => int.Parse(Value[..^1]);
    public char Column => Value[^1];

    [GeneratedRegex(@"^[1-9][0-9]?[A-F]$", RegexOptions.IgnoreCase)]
    private static partial Regex ValidSeatNumberRegex();
}
