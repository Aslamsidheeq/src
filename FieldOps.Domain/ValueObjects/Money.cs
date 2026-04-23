namespace FieldOps.Domain.ValueObjects;

public sealed record Money(decimal Amount, string Currency = "AED")
{
    public static Money Zero(string currency = "AED") => new(0m, currency);

    public Money Round2() => this with { Amount = decimal.Round(Amount, 2, MidpointRounding.AwayFromZero) };
}
