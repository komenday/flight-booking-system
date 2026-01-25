using System.Text.RegularExpressions;

namespace FBS.Domain.SharedKernel;

public partial record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        Value = value;
    }

    internal static PhoneNumber From(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone number cannot be empty", nameof(phone));

        var cleaned = new string([.. phone.Where(c => char.IsDigit(c) || c == '+')]);

        if (!PhoneRegex.IsMatch(cleaned))
            throw new ArgumentException(
                $"Invalid phone number format: {phone}. Expected E.164 format (e.g., +380501234567)",
                nameof(phone));

        return new PhoneNumber(cleaned);
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;

    [GeneratedRegex(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled)]
    private static partial Regex CreatePhoneRegex();

    private static readonly Regex PhoneRegex = CreatePhoneRegex();
}
