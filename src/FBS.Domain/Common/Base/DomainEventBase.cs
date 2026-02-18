using FBS.Domain.Common.Interfaces;

namespace FBS.Domain.Common.Base;

public abstract record ExternalDomainEventBase : IExternalDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}

public abstract record InternalDomainEventBase : IInternalDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
