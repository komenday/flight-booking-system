using FBS.Domain.Flight;

namespace FBS.Domain.Services;

public interface IFlightProviderService
{
    Task<IEnumerable<Seat>> GetSeatsForFlightAsync(FlightNumber flightNumber, CancellationToken cancellationToken);
}
