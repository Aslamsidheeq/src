using FieldOps.Domain.Enums;

namespace FieldOps.Domain.ValueObjects;

public sealed record Address(
    string Street,
    string Area,
    Emirates Emirate,
    string Country = "UAE");
