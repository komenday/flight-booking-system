using FBS.Domain.Common.Interfaces;
using FBS.Domain.Flight;

namespace FBS.Domain.Repositories;

public interface IFlightRepository
{
    ValueTask<Flight.Flight?> GetByIdAsync(FlightId id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Flight.Flight>> GetAsync(ISpecification<Flight.Flight> spec, CancellationToken cancellationToken);

    Task<Flight.Flight?> GetFirstOrDefaultAsync(ISpecification<Flight.Flight> spec, CancellationToken cancellationToken);

    ValueTask AddAsync(Flight.Flight flight, CancellationToken cancellationToken);

    ValueTask UpdateAsync(Flight.Flight flight, CancellationToken cancellationToken);
}