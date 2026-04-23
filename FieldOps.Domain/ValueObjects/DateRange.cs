namespace FieldOps.Domain.ValueObjects;

public sealed record DateRange(DateTime StartUtc, DateTime EndUtc)
{
    public TimeSpan Duration => EndUtc - StartUtc;
    public bool Overlaps(DateRange other) => StartUtc < other.EndUtc && EndUtc > other.StartUtc;
}
