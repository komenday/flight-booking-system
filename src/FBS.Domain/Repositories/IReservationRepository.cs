using FBS.Domain.Flight;
using FBS.Domain.Reservation;
using FBS.Domain.SharedKernel;

namespace FBS.Domain.Repositories;

public interface IReservationRepository
{
    Task<Reservation.Reservation?> GetByIdAsync(ReservationId id, CancellationToken cancellationToken);

    Task<IEnumerable<Reservation.Reservation>> GetByFlightIdAsync(FlightId flightId, CancellationToken cancellationToken);

    Task<IEnumerable<Reservation.Reservation>> GetExpiredReservationsAsync(CancellationToken cancellationToken);

    Task<IEnumerable<Reservation.Reservation>> GetByPassengerEmailAsync(Email email, CancellationToken cancellationToken);

    Task AddAsync(Reservation.Reservation reservation, CancellationToken cancellationToken);

    Task UpdateAsync(Reservation.Reservation reservation, CancellationToken cancellationToken);
}