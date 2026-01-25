using FBS.Domain.Common.Base;

namespace FBS.Domain.Flight.Events;

public record SeatReleasedEvent : DomainEventBase
{
    public SeatReleasedEvent(
        FlightId flightId,
        SeatNumber seatNumber)
    {
        FlightId = flightId;
        SeatNumber = seatNumber;
    }

    public FlightId FlightId { get; init; }

    public SeatNumber SeatNumber { get; init; }
}
