using FBS.Domain.Flight;
using FBS.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FBS.Infrastructure.Persistence.Configurations;

public class FlightConfiguration : IEntityTypeConfiguration<Flight>
{
    public void Configure(EntityTypeBuilder<Flight> builder)
    {
        builder.ToTable("Flights");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(
                id => id.Value,
                value => FlightId.From(value))
            .ValueGeneratedNever();

        builder.Property(f => f.Number)
            .HasConversion(
                fn => fn.Value,
                value => FlightNumber.From(value))
            .HasMaxLength(10)
            .IsRequired();

        builder.HasIndex(f => f.Number)
            .IsUnique();

        builder.Property(f => f.Departure)
            .HasConversion(
                a => a.IataCode,
                value => Airport.From(value))
            .HasMaxLength(3)
            .IsRequired()
            .HasColumnName("DepartureAirport");

        builder.Property(f => f.Arrival)
            .HasConversion(
                a => a.IataCode,
                value => Airport.From(value))
            .HasMaxLength(3)
            .IsRequired()
            .HasColumnName("ArrivalAirport");

        builder.Property(f => f.DepartureTime)
            .IsRequired();

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.HasMany(f => f.Seats)
            .WithOne()
            .HasForeignKey("FlightId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(f => f.DomainEvents);
    }
}