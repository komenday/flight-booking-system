using MediatR;

namespace FBS.Domain.Common.Interfaces;

public interface IDomainEvent : INotification
{
    Guid EventId { get; }

    DateTime OccurredAt { get; }
}

public interface IExternalDomainEvent : IDomainEvent
{
}

public interface IInternalDomainEvent : IDomainEvent
{
}