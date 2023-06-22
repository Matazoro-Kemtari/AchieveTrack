using Wada.AchieveTrackService.WorkRecordReader;
using Wada.AOP.Logging;
using Wada.ReadWorkRecordApplication;

namespace Wada.VerifyAchievementRecordContentApplication;

public record class WorkRecordParam(DateTime WorkingDate,
                                           uint EmployeeNumber,
                                           string WorkingNumber,
                                           decimal ManHour)
{
    [Logging]
    public static WorkRecordParam Parse(WorkRecordAttempt workRecordAttempt)
    {
        return new(workRecordAttempt.WorkingDate,
                   workRecordAttempt.EmployeeNumber,
                   workRecordAttempt.WorkingNumber,
                   workRecordAttempt.ManHour);
    }

    [Logging]
    public WorkRecord ConvertWorkRecord()
        => WorkRecord.Create(
            WorkingDate,
            EmployeeNumber,
            AchieveTrackService.ValueObjects.WorkingNumber.Create(WorkingNumber),
            AchieveTrackService.ValueObjects.ManHour.Create(ManHour));
}

public class TestAchievementRecordParamFactory
{
    [Logging]
    public static WorkRecordParam Create(DateTime? workingDate = default,
                                                uint employeeNumber = 4001u,
                                                string workingNumber = "23Z-1",
                                                decimal manHour = 4)
    {
        workingDate ??= new DateTime(2023, 4, 1);
        return new WorkRecordParam(workingDate.Value,
                                          employeeNumber,
                                          workingNumber,
                                          manHour);
    }
}