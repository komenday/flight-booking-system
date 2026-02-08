using FBS.Domain.Common.Interfaces;
using FBS.Domain.Reservation;

namespace FBS.Domain.Common.Rules;

public class ReservationMustBePendingToConfirmRule(ReservationId reservationId, ReservationStatus currentStatus)
    : IBusinessRule
{
    private readonly ReservationStatus _currentStatus = currentStatus;
    private readonly ReservationId _reservationId = reservationId;

    public bool IsBroken() => _currentStatus != ReservationStatus.Pending;

    public string Message => $"Cannot confirm reservation {_reservationId.Value} in status {_currentStatus}";

    public RuleErrorType ErrorType => RuleErrorType.Conflict;
}

public class ReservationMustNotBeExpiredToConfirmRule(ReservationId reservationId, DateTime expiresAt, DateTime? now = null)
    : IBusinessRule
{
    private readonly DateTime _expiresAt = expiresAt;
    private readonly DateTime _now = now ?? DateTime.UtcNow;
    private readonly ReservationId _reservationId = reservationId;

    public bool IsBroken() => _now > _expiresAt;

    public string Message => $"Reservation {_reservationId.Value} has expired at {_expiresAt:yyyy-MM-dd HH:mm:ss} UTC";

    public RuleErrorType ErrorType => RuleErrorType.Conflict;
}

public class CannotCancelExpiredOrCancelledReservationRule(ReservationId reservationId, ReservationStatus currentStatus)
    : IBusinessRule
{
    private readonly ReservationStatus _currentStatus = currentStatus;
    private readonly ReservationId _reservationId = reservationId;

    public bool IsBroken() =>
        _currentStatus == ReservationStatus.Expired ||
        _currentStatus == ReservationStatus.Cancelled;

    public string Message => $"Cannot cancel reservation {_reservationId.Value} in status {_currentStatus}";

    public RuleErrorType ErrorType => RuleErrorType.Conflict;
}

public class OnlyPendingReservationsCanExpireRule(ReservationStatus currentStatus)
    : IBusinessRule
{
    private readonly ReservationStatus _currentStatus = currentStatus;

    public bool IsBroken() => _currentStatus != ReservationStatus.Pending;

    public string Message => "Only Pending reservations can expire";

    public RuleErrorType ErrorType => RuleErrorType.Conflict;
}