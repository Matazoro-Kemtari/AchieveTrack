namespace Wada.AchieveTrackService.ValueObjects;

public record class WorkingNumber : Data.OrderManagement.Models.ValueObjects.WorkingNumber
{
    public WorkingNumber(string Value) : base(Value)
    { }

    public static WorkingNumber Create(string Value) => new(Value);
}

public class TestWorkingNumberFactory
{
    public static WorkingNumber Create(string value = "22Z-1") => new(value);
}