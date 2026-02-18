using FBS.Domain.Common.Base;

namespace FBS.Domain.Flight.Events;

public record SeatReservedEvent : InternalDomainEventBase
{
    public SeatReservedEvent(
        FlightId flightId,
        SeatNumber seatNumber)
    {
        FlightId = flightId;
        SeatNumber = seatNumber;
    }

    public FlightId FlightId { get; init; }

    public SeatNumber SeatNumber { get; init; }

}
