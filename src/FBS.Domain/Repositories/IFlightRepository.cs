using FBS.Domain.Common.Interfaces;
using FBS.Domain.Flight;
using FBS.Domain.SharedKernel;

namespace FBS.Domain.Repositories;

public interface IFlightRepository
{
    Task<Flight.Flight?> GetByIdAsync(FlightId id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Flight.Flight>> GetAsync(ISpecification<Flight.Flight> spec, CancellationToken cancellationToken);

    Task<Flight.Flight?> GetFirstOrDefaultAsync(ISpecification<Flight.Flight> spec, CancellationToken cancellationToken);

    Task AddAsync(Flight.Flight flight, CancellationToken cancellationToken);

    Task UpdateAsync(Flight.Flight flight, CancellationToken cancellationToken);
}