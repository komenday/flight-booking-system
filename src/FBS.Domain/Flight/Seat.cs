using FBS.Domain.Common.Base;

namespace FBS.Domain.Flight;

public class Seat : Entity<SeatNumber>
{
    public SeatNumber Number { get; private set; } = null!;

    public bool IsAvailable { get; private set; }

    private Seat() { }

    public static Seat Create(SeatNumber number)
    {
        return new Seat
        {
            Number = number,
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
