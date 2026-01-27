using FBS.Domain.Flight;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FBS.Infrastructure.Persistence.Configurations;

public class SeatConfiguration : IEntityTypeConfiguration<Seat>
{
    public void Configure(EntityTypeBuilder<Seat> builder)
    {
        builder.ToTable("Seats");

        builder.HasKey("Id");

        builder.Property(s => s.Number)
            .HasConversion(
                sn => sn.Value,
                value => SeatNumber.From(value))
            .HasMaxLength(5)
            .IsRequired();

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.IsAvailable)
            .IsRequired();

        builder.Property<FlightId>("FlightId")
            .HasConversion(
                id => id.Value,
                value => FlightId.From(value))
            .IsRequired();

        builder.HasIndex("FlightId", nameof(Seat.IsAvailable));
    }
}
