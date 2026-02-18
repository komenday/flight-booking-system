using FBS.Domain.Common.Interfaces;
using FBS.Domain.Flight;
using FBS.Domain.SharedKernel;

namespace FBS.Domain.Common.Rules;

public class AirportsMustBeDifferentRule(Airport departure, Airport arrival)
    : IBusinessRule
{
    private readonly Airport _departure = departure;
    private readonly Airport _arrival = arrival;

    public bool IsBroken() => _departure == _arrival;

    public string Message => "Departure and arrival airports must be different";

    public RuleErrorType ErrorType => RuleErrorType.Validation;
}

public class DepartureTimeMustBeInFutureRule(DateTime departureTime, DateTime? now = null)
    : IBusinessRule
{
    private readonly DateTime _departureTime = departureTime;
    private readonly DateTime _now = now ?? DateTime.UtcNow;

    public bool IsBroken() => _departureTime <= _now;

    public string Message => "Departure time must be in the future";

    public RuleErrorType ErrorType => RuleErrorType.Validation;
}

public class FlightMustHaveSeatsRule(int seatsCount)
    : IBusinessRule
{
    private readonly int _seatsCount = seatsCount;

    public bool IsBroken() => _seatsCount == 0;

    public string Message => "Flight must have at least one seat";

    public RuleErrorType ErrorType => RuleErrorType.Validation;
}

public class CannotReserveSeatOnDepartedFlightRule(DateTime departureTime, DateTime? now = null)
    : IBusinessRule
{
    private readonly DateTime _departureTime = departureTime;
    private readonly DateTime _now = now ?? DateTime.UtcNow;

    public bool IsBroken() => _now >= _departureTime;

    public string Message => "Cannot reserve seat on a flight that has already departed";

    public RuleErrorType ErrorType => RuleErrorType.Conflict;
}

public class SeatMustExistRule(Seat? seat, FlightNumber flightNumber, SeatNumber seatNumber) 
    : IBusinessRule
{
    private readonly Seat? _seat = seat;
    private readonly FlightNumber _flightNumber = flightNumber;
    private readonly SeatNumber _seatNumber = seatNumber;

    public bool IsBroken() => _seat is null;

    public string Message => string.Format("Seat {0} does not exist on flight {1}", _seatNumber.Value, _flightNumber.Value);

    public RuleErrorType ErrorType => RuleErrorType.NotFound;
}

public class SeatMustBeAvailableRule(Seat? seat, FlightNumber flightNumber, SeatNumber seatNumber)
    : IBusinessRule
{
    private readonly Seat? _seat = seat;
    private readonly FlightNumber _flightNumber = flightNumber;
    private readonly SeatNumber _seatNumber = seatNumber;

    public bool IsBroken() => _seat is null || !_seat.IsAvailable;

    public string Message => string.Format("Seat {0} on flight {1} is not available",
        _seatNumber.Value,
        _flightNumber.Value);

    public RuleErrorType ErrorType => RuleErrorType.Conflict;
}