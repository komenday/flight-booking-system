using FBS.Domain.Flight;
using FBS.Domain.Repositories;
using FBS.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence.Repositories;

public class FlightRepository(ApplicationDbContext context) : IFlightRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Flight?> GetByIdAsync(
        FlightId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Flights
            .Include(f => f.Seats)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<Flight?> GetByFlightNumberAsync(
        FlightNumber flightNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.Flights
            .Include(f => f.Seats)
            .FirstOrDefaultAsync(f => f.Number == flightNumber, cancellationToken);
    }

    public async Task<IEnumerable<Flight>> GetAvailableFlightsAsync(
        Airport departure,
        Airport arrival,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1);

        return await _context.Flights
            .Include(f => f.Seats)
            .Where(f =>
                f.Departure == departure &&
                f.Arrival == arrival &&
                f.DepartureTime >= startOfDay &&
                f.DepartureTime < endOfDay)
            .OrderBy(f => f.DepartureTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Flight flight,
        CancellationToken cancellationToken = default)
    {
        await _context.Flights.AddAsync(flight, cancellationToken);
    }

    public Task UpdateAsync(
        Flight flight,
        CancellationToken cancellationToken = default)
    {
        _context.Flights.Update(flight);
        return Task.CompletedTask;
    }
}
