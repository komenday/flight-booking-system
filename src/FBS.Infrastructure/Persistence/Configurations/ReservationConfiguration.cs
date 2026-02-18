using FBS.Domain.Flight;
using FBS.Domain.Reservation;
using FBS.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FBS.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => ReservationId.From(value))
            .ValueGeneratedNever();

        builder.Property(r => r.FlightId)
            .HasConversion(
                id => id.Value,
                value => FlightId.From(value))
            .IsRequired();

        builder.Property(r => r.FlightNumber)
            .HasConversion(
                fn => fn.Value,
                value => FlightNumber.From(value))
            .HasColumnName("FlightNumber")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(r => r.SeatNumber)
            .HasConversion(
                sn => sn.Value,
                value => SeatNumber.From(value))
            .HasMaxLength(5)
            .IsRequired();

        builder.OwnsOne(r => r.Passenger, passengerBuilder =>
        {
            passengerBuilder.Property(p => p.FirstName)
                .HasColumnName("PassengerFirstName")
                .HasMaxLength(50)
                .IsRequired();

            passengerBuilder.Property(p => p.LastName)
                .HasColumnName("PassengerLastName")
                .HasMaxLength(50)
                .IsRequired();

            passengerBuilder.Property(p => p.Email)
                .HasConversion(
                    e => e.Value,
                    value => Email.From(value))
                .HasColumnName("PassengerEmail")
                .HasMaxLength(254)
                .IsRequired();

            passengerBuilder.Property(p => p.Phone)
                .HasConversion(
                    phone => phone.Value,
                    value => PhoneNumber.From(value))
                .HasColumnName("PassengerPhone")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.Property(r => r.ConfirmedAt)
            .IsRequired(false);

        builder.HasIndex(r => r.FlightId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => new { r.Status, r.ExpiresAt });

        builder.Property<byte[]>("RowVersion")
            .IsRowVersion()
            .IsConcurrencyToken();

        builder.Ignore(r => r.DomainEvents);
    }
}
