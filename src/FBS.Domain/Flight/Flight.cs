using FBS.Domain.Common.Base;
using FBS.Domain.Flight.Events;
using FBS.Domain.Flight.Exceptions;
using FBS.Domain.SharedKernel;

namespace FBS.Domain.Flight;

public class Flight : AggregateRoot<FlightId>
{
    public FlightNumber Number { get; private set; } = null!;

    public Airport Departure { get; private set; } = null!;

    public Airport Arrival { get; private set; } = null!;

    public DateTime DepartureTime { get; private set; }


    private readonly List<Seat> _seats = [];
    public IReadOnlyCollection<Seat> Seats => _seats.AsReadOnly();

    private Flight() { }

    public static Flight Create(
        FlightNumber number,
        Airport departure,
        Airport arrival,
        DateTime departureTime,
        IEnumerable<Seat> seats)
    {
        if (departure == arrival)
            throw new ArgumentException("Departure and arrival airports must be different");

        if (departureTime <= DateTime.UtcNow)
            throw new ArgumentException("Departure time must be in the future");

        var seatsList = seats.ToList();
        if (seatsList.Count == 0)
            throw new ArgumentException("Flight must have at least one seat");

        var flight = new Flight
        {
            Id = FlightId.New(),
            Number = number,
            Departure = departure,
            Arrival = arrival,
            DepartureTime = departureTime
        };

        flight._seats.AddRange(seatsList);

        return flight;
    }

    public void ReserveSeat(SeatNumber seatNumber)
    {
        if (HasDeparted())
            throw new FlightAlreadyDepartedException(Id, DepartureTime);

        var seat = _seats.FirstOrDefault(s => s.Number == seatNumber)
            ?? throw new SeatNotFoundException(Number, seatNumber);

        if (!seat.IsAvailable)
            throw new SeatNotAvailableException(Number, seatNumber);

        seat.Reserve();

        AddDomainEvent(new SeatReservedEvent(Id, seatNumber));
    }

    public void ReleaseSeat(SeatNumber seatNumber)
    {
        var seat = _seats.FirstOrDefault(s => s.Number == seatNumber)
            ?? throw new SeatNotFoundException(Number, seatNumber);

        seat?.Release();

        AddDomainEvent(new SeatReleasedEvent(Id, seatNumber));
    }

    public bool IsSeatAvailable(SeatNumber seatNumber)
    {
        var seat = _seats.FirstOrDefault(s => s.Number == seatNumber);
        return seat?.IsAvailable ?? false;
    }

    public bool HasDeparted()
    {
        return DateTime.UtcNow >= DepartureTime;
    }

    public int GetAvailableSeatsCount()
    {
        return _seats.Count(s => s.IsAvailable);
    }

    public bool IsFull()
    {
        return !_seats.Any(s => s.IsAvailable);
    }
}
