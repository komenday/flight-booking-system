using FBS.Domain.Flight;
using FBS.Domain.SharedKernel;

namespace FBS.Domain.Repositories;

public interface IFlightRepository
{
    Task<Flight.Flight?> GetByIdAsync(FlightId id, CancellationToken cancellationToken);

    Task<Flight.Flight?> GetByFlightNumberAsync(FlightNumber flightNumber, CancellationToken cancellationToken);

    Task<IEnumerable<Flight.Flight>> GetAvailableFlightsAsync(Airport departure, Airport arrival, DateTime date, CancellationToken cancellationToken);

    Task AddAsync(Flight.Flight flight, CancellationToken cancellationToken);

    Task UpdateAsync(Flight.Flight flight, CancellationToken cancellationToken);
}
