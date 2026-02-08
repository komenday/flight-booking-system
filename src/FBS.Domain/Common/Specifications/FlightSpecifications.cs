using FBS.Domain.Flight;
using FBS.Domain.SharedKernel;

namespace FBS.Domain.Common.Specifications;

public class AvailableFlightsSpecification : Specification<Flight.Flight>
{
    public AvailableFlightsSpecification(Airport departure, Airport arrival, DateTime date)
        : base(f =>
            f.Departure == departure &&
            f.Arrival == arrival &&
            f.DepartureTime.Date == date.Date &&
            f.DepartureTime > DateTime.UtcNow)
    {
        AddInclude(f => f.Seats);
        ApplyOrderBy(f => f.DepartureTime);
    }
}

public class FlightByNumberWithSeatsSpecification : Specification<Flight.Flight>
{
    public FlightByNumberWithSeatsSpecification(FlightNumber flightNumber)
        : base(f => f.Number == flightNumber)
    {
        AddInclude(f => f.Seats);
    }
}