using FBS.Domain.Flight;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using FBS.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence.Repositories;

public class ReservationRepository(ApplicationDbContext context) : IReservationRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Reservation?> GetByIdAsync(
        ReservationId id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetByFlightIdAsync(
        FlightId flightId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Where(r => r.FlightId == flightId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync(
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _context.Reservations
            .Where(r =>
                r.Status == ReservationStatus.Pending &&
                r.ExpiresAt <= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Reservation>> GetByPassengerEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Where(r => EF.Property<string>(r.Passenger, "Email") == email.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
    }

    public Task UpdateAsync(
        Reservation reservation,
        CancellationToken cancellationToken = default)
    {
        _context.Reservations.Update(reservation);
        return Task.CompletedTask;
    }
}
