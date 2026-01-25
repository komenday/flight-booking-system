namespace FBS.Domain.Common.Interfaces;

public interface IDomainEvent
{
    Guid EventId { get; }

    DateTime OccurredAt { get; }
}