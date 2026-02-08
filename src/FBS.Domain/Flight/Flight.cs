using FBS.Domain.Common.Base;
using FBS.Domain.Common.Rules;
using FBS.Domain.Flight.Events;
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
        var seatsList = seats.ToList();
        var flight = new Flight();

        flight.CheckRules(
            new AirportsMustBeDifferentRule(departure, arrival),
            new DepartureTimeMustBeInFutureRule(departureTime),
            new FlightMustHaveSeatsRule(seatsList.Count)
        );

        flight.Id = FlightId.New();
        flight.Number = number;
        flight.Departure = departure;
        flight.Arrival = arrival;
        flight.DepartureTime = departureTime;
        flight._seats.AddRange(seatsList);

        return flight;
    }

    public void ReserveSeat(SeatNumber seatNumber)
    {
        var seat = _seats.FirstOrDefault(s => s.Number == seatNumber);

        this.CheckRules(
            new CannotReserveSeatOnDepartedFlightRule(DepartureTime),
            new SeatMustExistRule(seat is not null, Number, seatNumber),
            new SeatMustBeAvailableRule(seat?.IsAvailable ?? false, Number, seatNumber)
        );

        seat!.Reserve();

        AddDomainEvent(new SeatReservedEvent(Id, seatNumber));
    }

    public void ReleaseSeat(SeatNumber seatNumber)
    {
        var seat = _seats.FirstOrDefault(s => s.Number == seatNumber);

        this.CheckRule(new SeatMustExistRule(seat is not null, Number, seatNumber));

        seat!.Release();

        AddDomainEvent(new SeatReleasedEvent(Id, seatNumber));
    }

    public bool IsSeatAvailable(SeatNumber seatNumber)
    {
        var seat = _seats.FirstOrDefault(s => s.Number == seatNumber);
        return seat?.IsAvailable ?? false;
    }

    public bool HasDeparted(DateTime? asOf = null)
    {
        var now = asOf ?? DateTime.UtcNow;
        return now >= DepartureTime;
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
