using Wada.AchieveTrackService.ValueObjects;
using Wada.AOP.Logging;

namespace Wada.AchieveTrackService.AchieveTrackReader;

public record class WorkRecord
{
    private WorkRecord(DateTime workingDate, uint employeeNumber, WorkingNumber workingNumber, ManHour manHour)
    {
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        WorkingNumber = workingNumber ?? throw new ArgumentNullException(nameof(workingNumber));
        ManHour = manHour ?? throw new ArgumentNullException(nameof(manHour));
    }

    [Logging]
    public static WorkRecord Create(DateTime workingDate, uint employeeNumber, WorkingNumber workingNumber, ManHour manHour)
        => new(workingDate, employeeNumber, workingNumber, manHour);

    [Logging]
    public static WorkRecord Reconstruct(DateTime workingDate, uint employeeNumber, WorkingNumber workingNumber, ManHour manHour)
        => Create(workingDate, employeeNumber, workingNumber, manHour);

    public DateTime WorkingDate { get; init; }

    public uint EmployeeNumber { get; init; }

    public WorkingNumber WorkingNumber { get; init; }

    public ManHour ManHour { get; init; }
}

public class TestWorkRecordFactory
{
    [Logging]
    public static WorkRecord Create(DateTime? workingDate = default,
                                    uint employeeNumber = 4001u,
                                    WorkingNumber? workingNumber = default,
                                    ManHour? manHour = default)
    {
        workingDate ??= new DateTime(2023, 4, 1);
        workingNumber ??= TestWorkingNumberFactory.Create("23Z-1");
        manHour ??= ManHour.Create(4);
        return WorkRecord.Reconstruct(workingDate.Value, employeeNumber, workingNumber, manHour);
    }
}