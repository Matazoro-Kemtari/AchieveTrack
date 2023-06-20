using Wada.AchieveTrackService.ValueObjects;
using Wada.AOP.Logging;

namespace Wada.AchieveTrackService.AchieveTrackReader;

public record class WorkRecord
{
    private const decimal minimumManHour = 0.02m;

    private WorkRecord(DateTime workingDate, uint employeeNumber, WorkingNumber workingNumber, decimal manHour)
    {
        if (manHour < minimumManHour)
            throw new DomainException($"工数は最小値({minimumManHour:F2})より大きい値にしてください 工数: {manHour:F2}");
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        WorkingNumber = workingNumber ?? throw new ArgumentNullException(nameof(workingNumber));
        ManHour = manHour;
    }

    [Logging]
    public static WorkRecord Create(DateTime workingDate, uint employeeNumber, WorkingNumber workingNumber, decimal manHour)
        => new(workingDate, employeeNumber, workingNumber, manHour);

    [Logging]
    public static WorkRecord Reconstruct(DateTime workingDate, uint employeeNumber, WorkingNumber workingNumber, decimal manHour)
        => Create(workingDate, employeeNumber, workingNumber, manHour);

    public DateTime WorkingDate { get; init; }

    public uint EmployeeNumber { get; init; }

    public WorkingNumber WorkingNumber { get; init; }

    public decimal ManHour { get; init; }
}

public class TestWorkRecordFactory
{
    [Logging]
    public static WorkRecord Create(DateTime? workingDate = default,
                                    uint employeeNumber = 4001u,
                                    WorkingNumber? workingNumber = default,
                                    decimal manHour = 4)
    {
        workingDate ??= new DateTime(2023, 4, 1);
        workingNumber ??=TestWorkingNumberFactory.Create("23Z-1");
        return WorkRecord.Reconstruct(workingDate.Value, employeeNumber, workingNumber, manHour);
    }
}