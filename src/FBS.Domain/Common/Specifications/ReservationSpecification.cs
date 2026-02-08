using FBS.Domain.Flight;
using FBS.Domain.Reservation;
using FBS.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace FBS.Domain.Common.Specifications;

public class ExpiredReservationsSpecification : Specification<Reservation.Reservation>
{
    public ExpiredReservationsSpecification()
        : base(r =>
            r.Status == ReservationStatus.Pending &&
            r.ExpiresAt <= DateTime.UtcNow)
    {
    }
}

public class ReservationsByFlightSpecification(FlightId flightId)
    : Specification<Reservation.Reservation>(r => r.FlightId == flightId)
{
}

public class ReservationsByPassengerEmailSpecification(Email email)
    : Specification<Reservation.Reservation>(r => EF.Property<string>(r.Passenger, "Email") == email.Value)
{
}