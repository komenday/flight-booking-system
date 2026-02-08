using FBS.Domain.Common.Interfaces;
using FBS.Domain.Flight;

namespace FBS.Domain.Common.Rules;

public class SeatMustBeAvailableToReserveRule(bool isAvailable, SeatNumber seatNumber)
    : IBusinessRule
{
    private readonly bool _isAvailable = isAvailable;
    private readonly SeatNumber _seatNumber = seatNumber;

    public bool IsBroken() => !_isAvailable;

    public string Message => $"Seat {_seatNumber.Value} is already reserved";

    public RuleErrorType ErrorType => RuleErrorType.Conflict;
}
