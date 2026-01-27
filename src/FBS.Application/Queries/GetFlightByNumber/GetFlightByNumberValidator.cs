using FluentValidation;

namespace FBS.Application.Queries.GetFlightByNumber;

public class GetFlightByNumberValidator : AbstractValidator<GetFlightByNumberQuery>
{
    public GetFlightByNumberValidator()
    {
        RuleFor(x => x.FlightNumber)
            .NotEmpty()
            .WithMessage("Flight number is required")
            .Matches(@"^[A-Z]{2}\d{3,4}$")
            .WithMessage("Invalid flight number format. E.g. UA123, BA4567");
    }
}