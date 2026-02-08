using FBS.Domain.Common.Interfaces;
using FBS.Domain.Repositories;
using FBS.Domain.Reservation;
using FBS.Infrastructure.Persistence.Specifications;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence.Repositories;

public class ReservationRepository(ApplicationDbContext context) : IReservationRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken)
    {
        return await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
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

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
    }

    public Task UpdateAsync(Reservation reservation, CancellationToken cancellationToken)
    {
        _context.Reservations.Update(reservation);
        return Task.CompletedTask;
    }
}
