namespace FBS.Domain.SharedKernel;

public record Airport
{
    public string IataCode { get; }

    private Airport(string iataCode)
    {
        if (string.IsNullOrWhiteSpace(iataCode))
            throw new ArgumentException("IATA code is required");

        if (iataCode.Length != 3)
            throw new ArgumentException("IATA code must be 3 characters");

        IataCode = iataCode.ToUpper();
    }

    public static Airport From(string iataCode) => new(iataCode);

    public override string ToString() => IataCode;
}
