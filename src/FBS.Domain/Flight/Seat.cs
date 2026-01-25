using FBS.Domain.Common.Base;

namespace FBS.Domain.Flight;

public class Seat : Entity<SeatNumber>
{
    public SeatNumber Number { get; private set; } = null!;

    public SeatType Type { get; private set; }

    public bool IsAvailable { get; private set; }

    private Seat() { }

    internal static Seat Create(SeatNumber number, SeatType type)
    {
        return new Seat
        {
            Number = number,
            Type = type,
            IsAvailable = true
        };
    }

    internal void Reserve()
    {
        if (!IsAvailable)
            throw new InvalidOperationException("Seat already reserved");

        IsAvailable = false;
    }

    internal void Release()
    {
        IsAvailable = true;
    }
}
