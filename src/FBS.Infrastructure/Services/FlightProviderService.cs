using FBS.Domain.Flight;
using FBS.Domain.Services;

namespace FBS.Infrastructure.Services;

public class FlightProviderService : IFlightProviderService
{
    public Task<IEnumerable<Seat>> GetSeatsForFlightAsync(
        FlightNumber flightNumber,
        CancellationToken cancellationToken = default)
    {
        //Stub implementation - generates seats based on flight number
        var totalSeats = DetermineSeatsCountByFlightNumber(flightNumber);
        var seats = GenerateSeats(totalSeats).AsEnumerable();

        return Task.FromResult(seats);
    }

    private static int DetermineSeatsCountByFlightNumber(FlightNumber flightNumber)
    {
        // Simple logic: different airlines have different capacities
        var airline = flightNumber.Airline;

        return airline switch
        {
            "UA" => 180,
            "BA" => 150,
            "LH" => 200,
            _ => 180
        };
    }

    private static List<Seat> GenerateSeats(int totalSeats)
    {
        const int seatsPerRow = 6;
        var rows = (int)Math.Ceiling(totalSeats / (double)seatsPerRow);

        var seats = new List<Seat>();

        for (int row = 1; row <= rows; row++)
        {
            for (char column = 'A'; column < 'A' + seatsPerRow; column++)
            {
                if (seats.Count >= totalSeats)
                    break;

                var seatNumber = SeatNumber.From($"{row}{column}");
                seats.Add(Seat.Create(seatNumber));
            }
        }

        return seats;
    }
}
