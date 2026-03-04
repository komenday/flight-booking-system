using FBS.Domain.Common.Interfaces;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using FBS.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence.Repositories;

public class ReservationRepository(ApplicationDbContext context) : IReservationRepository
{
    private readonly ApplicationDbContext _context = context;

    public async ValueTask<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken)
    {
        var localReservation = _context.Reservations.Local.FirstOrDefault(r => r.Id == id);
        if (localReservation is not null)
        {
            return localReservation;
        }

        return await _context.Reservations.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<Reservation>> GetAsync(ISpecification<Reservation> spec, CancellationToken cancellationToken)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Reservations, spec);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Reservation?> GetFirstOrDefaultAsync(ISpecification<Reservation> spec, CancellationToken cancellationToken)
    {
        var query = SpecificationEvaluator.GetQuery(_context.Reservations, spec);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask AddAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
    }

    public ValueTask UpdateAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        _context.Reservations.Update(reservation);
        return ValueTask.CompletedTask;
    }
}
