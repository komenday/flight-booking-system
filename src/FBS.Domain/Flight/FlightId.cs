namespace FBS.Domain.Flight;

public record FlightId(Guid Value)
{
    public static FlightId New() => new(Guid.NewGuid());

    public static FlightId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
