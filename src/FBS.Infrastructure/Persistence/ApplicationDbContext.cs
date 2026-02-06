using FBS.Application.Common.Interfaces;
using FBS.Domain.Common.Interfaces;
using FBS.Domain.Flight;
using FBS.Domain.Reservation;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventDispatcher domainEventDispatcher)
    : DbContext(options)
{
    private readonly IDomainEventDispatcher? _domainEventDispatcher = domainEventDispatcher;

    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    // Constructor for EF Core Design-time tools (migrations)
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : this(options, null!)
    {
        _domainEventDispatcher = null;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        if (_domainEventDispatcher is not null)
        {
            await DispatchDomainEventsAsync(cancellationToken);
        }

        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var aggregatesWithEvents = ChangeTracker
            .Entries()
            .Where(e => e.Entity is IAggregateRoot aggregate && aggregate.DomainEvents.Count != 0)
            .Select(e => (IAggregateRoot)e.Entity)
            .ToList();

        var domainEvents = aggregatesWithEvents
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList();


        aggregatesWithEvents.ForEach(aggregate => aggregate.ClearDomainEvents());

        await _domainEventDispatcher!.DispatchAsync(domainEvents, cancellationToken);
    }
}
