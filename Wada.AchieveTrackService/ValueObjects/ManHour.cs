namespace Wada.AchieveTrackService.ValueObjects;

public record class ManHour
{
    private const decimal minimumManHour = 0.02m;

    private ManHour(decimal value)
    {
        if (value < minimumManHour)
            throw new DomainException($"工数は最小値({minimumManHour:F2})より大きい値にしてください 工数: {value:F2}");

        Value = value;
    }

    public static ManHour Create(decimal value) =>new(value);

    public static ManHour Reconstruct(decimal value) => new(value);

    public override string ToString() => Value.ToString();

    public decimal Value { get; init; }
}

public class TestManHourFactory
{
    public static ManHour Create(decimal value = 9.25m)
        => ManHour.Reconstruct(value);
}