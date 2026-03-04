using FBS.Domain.Common.Interfaces;
using FBS.Domain.Reservation;

namespace FBS.Domain.Repositories;

public interface IReservationRepository
{
    ValueTask<Reservation.Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Reservation.Reservation>> GetAsync(ISpecification<Reservation.Reservation> spec, CancellationToken cancellationToken);

    Task<Reservation.Reservation?> GetFirstOrDefaultAsync(ISpecification<Reservation.Reservation> spec, CancellationToken cancellationToken);

    ValueTask AddAsync(Reservation.Reservation reservation, CancellationToken cancellationToken);

    ValueTask UpdateAsync(Reservation.Reservation reservation, CancellationToken cancellationToken);
}