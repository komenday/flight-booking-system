using FBS.Domain.Common.Interfaces;
using FBS.Domain.Flight;
using FBS.Domain.Repositories;
using FBS.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence.Repositories;

public class FlightRepository(ApplicationDbContext context) : IFlightRepository
{
    private readonly ApplicationDbContext _context = context;

    public async ValueTask<Flight?> GetByIdAsync(FlightId id, CancellationToken cancellationToken)
    {
        var localFlight = _context.Flights.Local.FirstOrDefault(f => f.Id == id);
        if (localFlight is not null)
        {
            return localFlight;
        }

        var flight = await _context.Flights.FindAsync([id], cancellationToken);
        if (flight is not null)
        {
            await _context.Entry(flight)
                .Collection(f => f.Seats)
                .LoadAsync(cancellationToken);
        }

        return flight;
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

    public async ValueTask AddAsync(Flight flight, CancellationToken cancellationToken)
    {
        await _context.Flights.AddAsync(flight, cancellationToken);
    }

    public ValueTask UpdateAsync(Flight flight, CancellationToken cancellationToken)
    {
        _context.Flights.Update(flight);
        return ValueTask.CompletedTask;
    }
}
