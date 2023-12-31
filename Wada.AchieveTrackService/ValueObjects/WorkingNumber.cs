﻿namespace Wada.AchieveTrackService.ValueObjects;

public record class WorkingNumber : Data.OrderManagement.Models.ValueObjects.WorkingNumber
{
    private WorkingNumber(string value) : base(value)
    { }

    public static WorkingNumber Create(string Value) => new(Value);

    public static WorkingNumber Reconstruct(string Value) => new(Value);

    public Data.OrderManagement.Models.ValueObjects.WorkingNumber Convert()
        => new(Value);

    public static WorkingNumber Parse(Data.OrderManagement.Models.ValueObjects.WorkingNumber workingNumber)
        => Create(workingNumber.Value);
}

public class TestWorkingNumberFactory
{
    public static WorkingNumber Create(string value = "22Z-1")
        => WorkingNumber.Reconstruct(value);
}