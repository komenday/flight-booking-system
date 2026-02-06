using FluentValidation;

namespace FBS.Application.Commands.ExpireReservation;

public class ExpireReservationValidator : AbstractValidator<ExpireReservationCommand>
{
    public ExpireReservationValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("Reservation ID is required");
    }
}