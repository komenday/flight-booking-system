using FBS.Domain.Common.Interfaces;

namespace FBS.Infrastructure.Events.Mapping;

public interface IEventMapper
{
    object MapToDto(IExternalDomainEvent domainEvent);
}