using FBS.Domain.SharedKernel;

namespace FBS.Domain.Reservation;

public record PassengerInfo
{
    public string FirstName { get; }

    public string LastName { get; }

    public Email Email { get; }

    public PhoneNumber Phone { get; }

    private PassengerInfo(
        string firstName,
        string lastName,
        Email email,
        PhoneNumber phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email;
        Phone = phone;
    }

    public static PassengerInfo Create(
        string firstName,
        string lastName,
        string email,
        string phone)
    {
        return new PassengerInfo(
            firstName,
            lastName,
            Email.From(email),
            PhoneNumber.From(phone)
        );
    }

    public string FullName => $"{FirstName} {LastName}";
}
