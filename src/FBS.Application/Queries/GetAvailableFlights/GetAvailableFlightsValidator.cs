using FluentValidation;

namespace FBS.Application.Queries.GetAvailableFlights;

public class GetAvailableFlightsValidator : AbstractValidator<GetAvailableFlightsQuery>
{
    public GetAvailableFlightsValidator()
    {
        RuleFor(x => x.DepartureAirport)
            .NotEmpty()
            .WithMessage("Departure airport is required")
            .Length(3)
            .WithMessage("Airport code must be 3 characters (IATA code)")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Invalid airport code format");

        RuleFor(x => x.ArrivalAirport)
            .NotEmpty()
            .WithMessage("Arrival airport is required")
            .Length(3)
            .WithMessage("Airport code must be 3 characters (IATA code)")
            .Matches(@"^[A-Z]{3}$")
            .WithMessage("Invalid airport code format");

        RuleFor(x => x.DepartureAirport)
            .NotEqual(x => x.ArrivalAirport)
            .WithMessage("Departure and arrival airports must be different");

        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Date must be today or in the future");
    }
}
