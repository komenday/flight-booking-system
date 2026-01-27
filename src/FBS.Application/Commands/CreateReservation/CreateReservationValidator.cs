using FluentValidation;

namespace FBS.Application.Commands.CreateReservation;

public class CreateReservationValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.FlightId)
            .NotEmpty()
            .WithMessage("Flight ID is required");

        RuleFor(x => x.SeatNumber)
            .NotEmpty()
            .WithMessage("Seat number is required")
            .Matches(@"^[1-9][0-9]?[A-F]$")
            .WithMessage("Invalid seat number format");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("First name contains invalid characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("Last name contains invalid characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format")
            .MaximumLength(254)
            .WithMessage("Email must not exceed 254 characters");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .WithMessage("Invalid phone number format. Expected E.164 format");
    }
}