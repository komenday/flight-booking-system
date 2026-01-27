using FluentValidation;

namespace FBS.Application.Queries.GetReservation;

public class GetReservationValidator : AbstractValidator<GetReservationQuery>
{
    public GetReservationValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("Reservation ID is required");
    }
}
