using Bogus;
using FBS.Domain.Flight;
using FBS.Domain.SharedKernel;
using FBS.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace FBS.Infrastructure.Seed;

public class FlightDataSeeder(ApplicationDbContext context, ILogger<FlightDataSeeder> logger)
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<FlightDataSeeder> _logger = logger;

    private static readonly string[] Airports 
        = ["JFK", "LAX", "ORD", "DFW", "DEN", "SFO", "SEA", "LAS", "MCO", "MIA", "ATL", "BOS", "PHX"];

    private static readonly string[] Airlines 
        = ["AA", "UA", "DL", "SW", "B6", "AS", "NK", "F9"];

    public async Task SeedAsync(int count = 15, CancellationToken cancellationToken = default)
    {
        if (_context.Flights.Any())
        {
            _logger.LogInformation("Database already contains flights. Skipping seed.");
            return;
        }

        _logger.LogInformation("Starting flight data seeding. Creating {Count} flights...", count);

        var faker = new Faker();
        var flights = new List<Flight>();

        for (int i = 0; i < count; i++)
        {
            try
            {
                var flight = GenerateFlight(faker);
                flights.Add(flight);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to generate flight #{Number}", i + 1);
            }
        }

        await _context.Flights.AddRangeAsync(flights, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully seeded {Count} flights with {SeatCount} total seats", flights.Count, flights.Sum(f => f.Seats.Count));
    }

    private Flight GenerateFlight(Faker faker)
    {
        string departureAirport, arrivalAirport;
        do
        {
            departureAirport = faker.PickRandom(Airports);
            arrivalAirport = faker.PickRandom(Airports);
        }
        while (departureAirport == arrivalAirport);

        var airline = faker.PickRandom(Airlines);
        var flightNumberValue = $"{airline}{faker.Random.Number(100, 999)}";
        var flightNumber = FlightNumber.From(flightNumberValue);

        var departureTime = faker.Date
            .Between(
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(30)
            )
            .AddHours(faker.Random.Number(6, 22));

        var seats = GenerateSeats(faker);

        var flight = Flight.Create(
            flightNumber,
            Airport.From(departureAirport),
            Airport.From(arrivalAirport),
            departureTime,
            seats
        );

        _logger.LogDebug("Generated flight: {FlightNumber} {Departure} -> {Arrival} at {DepartureTime}", flightNumberValue, departureAirport, arrivalAirport, departureTime);

        return flight;
    }

    private static List<Seat> GenerateSeats(Faker faker)
    {
        var seats = new List<Seat>();

        // Economy
        for (int row = 10; row <= 30; row++)
        {
            foreach (var letter in new[] { "A", "B", "C", "D", "E", "F" })
            {
                var seatNumber = SeatNumber.From($"{row}{letter}");
                seats.Add(Seat.Create(seatNumber));
            }
        }

        // Business
        for (int row = 1; row <= 5; row++)
        {
            foreach (var letter in new[] { "A", "B", "C", "D" })
            {
                var seatNumber = SeatNumber.From($"{row}{letter}");
                seats.Add(Seat.Create(seatNumber));
            }
        }

        // Randomly reserve 10-30% seats
        var seatsToReserve = faker.Random.Number((int)(seats.Count * 0.1), (int)(seats.Count * 0.3));
        var randomSeats = faker.PickRandom(seats, seatsToReserve);

        foreach (var seat in randomSeats)
        {
            var property = typeof(Seat).GetProperty(nameof(Seat.IsAvailable));
            property?.SetValue(seat, false);
        }

        return seats;
    }
}
