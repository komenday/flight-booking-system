using FBS.Domain.Reservation.Events;
using FBS.Infrastructure.Events.DTOs;

namespace FBS.Infrastructure.Events.Mapping;
public static class EventMapper
{
    public static ReservationCreatedEventDto ToDto(this ReservationCreatedEvent domainEvent)
    {
        return new ReservationCreatedEventDto
        {
            EventId = domainEvent.EventId,
            OccurredAt = domainEvent.OccurredAt,
            ReservationId = domainEvent.ReservationId.Value,        
            FlightId = domainEvent.FlightId.Value,                  
            FlightNumber = domainEvent.FlightNumber.Value,          
            SeatNumber = domainEvent.SeatNumber.Value,             
            PassengerFirstName = domainEvent.Passenger.FirstName,   
            PassengerLastName = domainEvent.Passenger.LastName,
            PassengerEmail = domainEvent.Passenger.Email.Value,
            ExpiresAt = domainEvent.ExpiresAt
        };
    }

    public static ReservationConfirmedEventDto ToDto(this ReservationConfirmedEvent domainEvent)
    {
        return new ReservationConfirmedEventDto
        {
            EventId = domainEvent.EventId,
            OccurredAt = domainEvent.OccurredAt,
            ReservationId = domainEvent.ReservationId.Value,
            FlightId = domainEvent.FlightId.Value,
            FlightNumber = domainEvent.FlightNumber.Value,
            SeatNumber = domainEvent.SeatNumber.Value,
            PassengerFirstName = domainEvent.Passenger.FirstName,
            PassengerLastName = domainEvent.Passenger.LastName,
            PassengerEmail = domainEvent.Passenger.Email.Value
        };
    }

    public static ReservationCancelledEventDto ToDto(this ReservationCancelledEvent domainEvent)
    {
        return new ReservationCancelledEventDto
        {
            EventId = domainEvent.EventId,
            OccurredAt = domainEvent.OccurredAt,
            ReservationId = domainEvent.ReservationId.Value,
            FlightId = domainEvent.FlightId.Value,
            FlightNumber = domainEvent.FlightNumber.Value,
            SeatNumber = domainEvent.SeatNumber.Value,
            PassengerFirstName = domainEvent.Passenger.FirstName,
            PassengerLastName = domainEvent.Passenger.LastName,
            PassengerEmail = domainEvent.Passenger.Email.Value
        };
    }

    public static ReservationExpiredEventDto ToDto(this ReservationExpiredEvent domainEvent)
    {
        return new ReservationExpiredEventDto
        {
            EventId = domainEvent.EventId,
            OccurredAt = domainEvent.OccurredAt,
            ReservationId = domainEvent.ReservationId.Value,
            FlightId = domainEvent.FlightId.Value,
            FlightNumber = domainEvent.FlightNumber.Value,
            SeatNumber = domainEvent.SeatNumber.Value,
            PassengerFirstName = domainEvent.Passenger.FirstName,
            PassengerLastName = domainEvent.Passenger.LastName,
            PassengerEmail = domainEvent.Passenger.Email.Value
        };
    }
}