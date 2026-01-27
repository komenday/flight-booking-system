using FluentValidation;

namespace FBS.Application.Commands.ConfirmReservation;

public class ConfirmReservationValidator : AbstractValidator<ConfirmReservationCommand>
{
    public ConfirmReservationValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("Reservation ID is required");
    }
}