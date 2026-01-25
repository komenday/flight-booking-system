using System.Text.RegularExpressions;

namespace FBS.Domain.SharedKernel;

public partial record Email
{
    public string Value { get; }

    public Email(string value)
    {
        Value = value;
    }

    internal static Email From(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        var normalized = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(normalized))
            throw new ArgumentException($"Invalid email format: {email}", nameof(email));

        if (normalized.Length > 254)
            throw new ArgumentException("Email is too long (max 254 characters)", nameof(email));

        return new Email(normalized);
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex CreateEmailRegex();

    private static readonly Regex EmailRegex = CreateEmailRegex();
}
