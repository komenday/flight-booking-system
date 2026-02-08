using FBS.Domain.Common.Interfaces;
using FBS.Domain.Reservation;
namespace FBS.Domain.Repositories;

public interface IReservationRepository
{
    Task<Reservation.Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken);

    Task<IReadOnlyList<Reservation.Reservation>> GetAsync(ISpecification<Reservation.Reservation> spec, CancellationToken cancellationToken);

    Task<Reservation.Reservation?> GetFirstOrDefaultAsync(ISpecification<Reservation.Reservation> spec, CancellationToken cancellationToken);

    Task AddAsync(Reservation.Reservation reservation, CancellationToken cancellationToken);

    Task UpdateAsync(Reservation.Reservation reservation, CancellationToken cancellationToken);
}