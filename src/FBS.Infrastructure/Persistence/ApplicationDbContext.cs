using FBS.Domain.Flight;
using FBS.Domain.Reservation;
using FBS.Infrastructure.EventDispatcher;
using Microsoft.EntityFrameworkCore;

namespace FBS.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    Lazy<IDomainEventDispatcher> domainEventDispatcher) : DbContext(options)
{
    private readonly Lazy<IDomainEventDispatcher>? _domainEventDispatcher = domainEventDispatcher;

    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    // Constructor for EF Core migrations
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
        await _domainEventDispatcher!.Value.DispatchEventsAsync(cancellationToken);
        return result;
    }
}
