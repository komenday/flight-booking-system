using FBS.Application.Common.Constants;
using FluentValidation;

namespace FBS.Application.Commands.CreateReservation;

public class CreateReservationValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.FlightNumber)
            .NotEmpty()
            .WithMessage("Flight number is required");

        RuleFor(x => x.SeatNumber)
            .NotEmpty()
            .WithMessage("Seat number is required")
            .Matches(@"^[1-9][0-9]?[A-F]$")
            .WithMessage("Invalid seat number format");

        RuleFor(x => x.Passenger)
            .NotNull()
            .WithMessage("Passenger information is required")
            .SetValidator(new PassengerInfoDtoValidator());
    }
}

public class PassengerInfoDtoValidator : AbstractValidator<PassengerInfoDto>
{
    public PassengerInfoDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(ValidationMessages.NameRequired)
            .MaximumLength(50)
            .WithMessage(string.Format(ValidationMessages.NameTooLong, 50))
            .Matches(ValidationPatterns.Name)
            .WithMessage(ValidationMessages.NameInvalid);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(ValidationMessages.NameRequired)
            .MaximumLength(50)
            .WithMessage(string.Format(ValidationMessages.NameTooLong, 50))
            .Matches(ValidationPatterns.Name)
            .WithMessage(ValidationMessages.NameInvalid);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(ValidationMessages.EmailRequired)
            .EmailAddress()
            .WithMessage(ValidationMessages.EmailInvalid)
            .MaximumLength(254)
            .WithMessage("Email must not exceed 254 characters");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage(ValidationMessages.PhoneRequired)
            .Matches(ValidationPatterns.Phone)
            .WithMessage(ValidationMessages.PhoneInvalid);
    }
}