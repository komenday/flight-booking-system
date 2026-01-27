using FluentValidation;

namespace FBS.Application.Commands.CancelReservation;

public class CancelReservationValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("Reservation ID is required");
    }
}
