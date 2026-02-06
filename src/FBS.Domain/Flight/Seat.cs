using FBS.Domain.Common.Base;

namespace FBS.Domain.Flight;

public class Seat : Entity<Guid>
{
    public SeatNumber Number { get; private set; } = null!;

    public bool IsAvailable { get; private set; }

    private Seat() { }

    public static Seat Create(SeatNumber number)
    {
        return new Seat
        {
            Id = Guid.NewGuid(),
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

    public override bool Equals(object? obj)
    {
        if (obj is not Seat other)
            return false;

        return Number == other.Number;
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }
}
