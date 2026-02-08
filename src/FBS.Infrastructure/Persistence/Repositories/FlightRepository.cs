using FBS.Domain.Common.Interfaces;
using FBS.Domain.Flight;
using FBS.Domain.Repositories;
using FBS.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence.Repositories;

public class FlightRepository(ApplicationDbContext context) : IFlightRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Flight?> GetByIdAsync(FlightId id, CancellationToken cancellationToken)
    {
        return await _context.Flights
            .Include(f => f.Seats)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Flight>> GetAsync(ISpecification<Flight> spec, CancellationToken cancellationToken)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Flights, spec);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Flight?> GetFirstOrDefaultAsync(ISpecification<Flight> spec, CancellationToken cancellationToken)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Flights, spec);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Flight flight, CancellationToken cancellationToken)
    {
        await _context.Flights.AddAsync(flight, cancellationToken);
    }

    public Task UpdateAsync(Flight flight, CancellationToken cancellationToken)
    {
        _context.Flights.Update(flight);
        return Task.CompletedTask;
    }
}
