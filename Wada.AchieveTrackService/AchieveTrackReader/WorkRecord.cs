using Wada.AOP.Logging;

namespace Wada.AchieveTrackService.AchieveTrackReader;

public record class WorkRecord
{
    private WorkRecord(DateTime workingDate, uint employeeNumber, string workingNumber, decimal manHour)
    {
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        WorkingNumber = workingNumber ?? throw new ArgumentNullException(nameof(workingNumber));
        ManHour = manHour;
    }

    [Logging]
    public static WorkRecord Create(DateTime workingDate, uint employeeNumber, string workingNumber, decimal manHour)
        => new(workingDate, employeeNumber, workingNumber, manHour);

    [Logging]
    public static WorkRecord Reconstruct(DateTime workingDate, uint employeeNumber, string workingNumber, decimal manHour)
        => Create(workingDate, employeeNumber, workingNumber, manHour);

    public DateTime WorkingDate { get; init; }

    public uint EmployeeNumber { get; init; }

    public string WorkingNumber { get; init; }

    public decimal ManHour { get; init; }
}

public class TestWorkRecordFactory
{
    [Logging]
    public static WorkRecord Create(DateTime? workingDate = default,
                                    uint employeeNumber = 4001u,
                                    string workingNumber = "23Z-1",
                                    decimal manHour = 4)
    {
        workingDate ??= new DateTime(2023, 4, 1);
        return WorkRecord.Reconstruct(workingDate.Value, employeeNumber, workingNumber, manHour);
    }
}