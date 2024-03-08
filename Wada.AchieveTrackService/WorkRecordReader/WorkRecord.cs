using Wada.AchieveTrackService.ValueObjects;
using Wada.AOP.Logging;

namespace Wada.AchieveTrackService.WorkRecordReader;

public record class WorkRecord
{
    private WorkRecord(DateTime workingDate,
                       uint employeeNumber,
                       string employeeName,
                       WorkingNumber workingNumber,
                       string? jigCode,
                       string achievementClassification,
                       string? note,
                       ManHour manHour)
    {
        Id = Ulid.NewUlid();
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        EmployeeName = employeeName;
        WorkingNumber = workingNumber ?? throw new ArgumentNullException(nameof(workingNumber));
        JigCode = jigCode;
        AchievementClassification = achievementClassification;
        Note = note;
        ManHour = manHour ?? throw new ArgumentNullException(nameof(manHour));
    }

    private WorkRecord(Ulid id,
                       DateTime workingDate,
                       uint employeeNumber,
                       string employeeName,
                       WorkingNumber workingNumber,
                       string? jigCode,
                       string achievementClassification,
                       string? note,
                       ManHour manHour)
    {
        Id = id;
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        EmployeeName = employeeName;
        WorkingNumber = workingNumber ?? throw new ArgumentNullException(nameof(workingNumber));
        JigCode = jigCode;
        AchievementClassification = achievementClassification;
        Note = note;
        ManHour = manHour ?? throw new ArgumentNullException(nameof(manHour));
    }

    [Logging]
    public static WorkRecord Create(DateTime workingDate,
                                    uint employeeNumber,
                                    string employeeName,
                                    WorkingNumber workingNumber,
                                    string? jigCode,
                                    string achievementClassification,
                                    string? note,
                                    ManHour manHour)
        => new(workingDate, employeeNumber, employeeName, workingNumber, jigCode, achievementClassification, note, manHour);

    [Logging]
    public static WorkRecord Reconstruct(Ulid id,
                                         DateTime workingDate,
                                         uint employeeNumber,
                                         string employeeName,
                                         WorkingNumber workingNumber,
                                         string jigCode,
                                         string achievementClassification,
                                         string note,
                                         ManHour manHour)
        => new(id, workingDate, employeeNumber, employeeName, workingNumber, jigCode, achievementClassification, note, manHour);

    public Ulid Id { get; init; }

    public DateTime WorkingDate { get; init; }

    public uint EmployeeNumber { get; init; }

    public string EmployeeName { get; init; }

    public WorkingNumber WorkingNumber { get; init; }

    public string? JigCode { get; init; }

    public string AchievementClassification { get; init; }

    public string? Note { get; init; }

    public ManHour ManHour { get; init; }
}

public class TestWorkRecordFactory
{
    [Logging]
    public static WorkRecord Create(DateTime? workingDate = default,
                                    uint employeeNumber = 4001u,
                                    string employeeName = "本社　無人",
                                    WorkingNumber? workingNumber = default,
                                    string jigCode = "11A",
                                    string achievementClassification = "NC",
                                    string note = "特記事項",
                                    ManHour? manHour = default)
    {
        workingDate ??= new DateTime(2023, 4, 1);
        workingNumber ??= TestWorkingNumberFactory.Create();
        manHour ??= ManHour.Create(4);
        return WorkRecord.Create(workingDate.Value, employeeNumber, employeeName, workingNumber, jigCode, achievementClassification, note, manHour);
    }
}