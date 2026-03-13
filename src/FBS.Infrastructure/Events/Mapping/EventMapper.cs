using FBS.Domain.Common.Interfaces;
using FBS.Domain.Reservation.Events;
using FBS.Infrastructure.Events.DTOs;

namespace FBS.Infrastructure.Events.Mapping;

public class EventMapper : IEventMapper
{
    private static readonly Dictionary<Type, Func<IExternalDomainEvent, object>> _mappers = [];

    static EventMapper()
    {
        Register<ReservationCreatedEvent>(e => new ReservationCreatedEventDto
        {
            EventId = e.EventId,
            OccurredAt = e.OccurredAt,
            ReservationId = e.ReservationId.Value,
            FlightId = e.FlightId.Value,
            FlightNumber = e.FlightNumber.Value,
            SeatNumber = e.SeatNumber.Value,
            PassengerFirstName = e.Passenger.FirstName,
            PassengerLastName = e.Passenger.LastName,
            PassengerEmail = e.Passenger.Email.Value,
            ExpiresAt = e.ExpiresAt
        });

        Register<ReservationConfirmedEvent>(e => new ReservationConfirmedEventDto
        {
            EventId = e.EventId,
            OccurredAt = e.OccurredAt,
            ReservationId = e.ReservationId.Value,
            FlightId = e.FlightId.Value,
            FlightNumber = e.FlightNumber.Value,
            SeatNumber = e.SeatNumber.Value,
            PassengerFirstName = e.Passenger.FirstName,
            PassengerLastName = e.Passenger.LastName,
            PassengerEmail = e.Passenger.Email.Value
        });

        Register<ReservationCancelledEvent>(e => new ReservationCancelledEventDto
        {
            EventId = e.EventId,
            OccurredAt = e.OccurredAt,
            ReservationId = e.ReservationId.Value,
            FlightId = e.FlightId.Value,
            FlightNumber = e.FlightNumber.Value,
            SeatNumber = e.SeatNumber.Value,
            PassengerFirstName = e.Passenger.FirstName,
            PassengerLastName = e.Passenger.LastName,
            PassengerEmail = e.Passenger.Email.Value
        });

        Register<ReservationExpiredEvent>(e => new ReservationExpiredEventDto
        {
            EventId = e.EventId,
            OccurredAt = e.OccurredAt,
            ReservationId = e.ReservationId.Value,
            FlightId = e.FlightId.Value,
            FlightNumber = e.FlightNumber.Value,
            SeatNumber = e.SeatNumber.Value,
            PassengerFirstName = e.Passenger.FirstName,
            PassengerLastName = e.Passenger.LastName,
            PassengerEmail = e.Passenger.Email.Value
        });
    }

    public object MapToDto(IExternalDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var type = domainEvent.GetType();
        if (!_mappers.TryGetValue(type, out var mapFunc))
            throw new InvalidOperationException($"Mapper was not found for an event of type {type.Name}");

        return mapFunc(domainEvent);
    }

    public static void Register<TEvent>(Func<TEvent, object> mapper) where TEvent : IExternalDomainEvent
    {
        _mappers[typeof(TEvent)] = e => mapper((TEvent)e);
    }
}