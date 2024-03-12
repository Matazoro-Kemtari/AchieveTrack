using Wada.AchieveTrackService.WorkRecordReader;
using Wada.AOP.Logging;
using Wada.ReadWorkRecordApplication;

namespace Wada.VerifyAchievementRecordContentApplication;

public record class WorkRecordParam(DateTime WorkingDate,
                                    uint EmployeeNumber,
                                    string EmployeeName,
                                    string WorkingNumber,
                                    string? JigCode,
                                    string ProcessFlow,
                                    string? Note,
                                    decimal ManHour)
{
    [Logging]
    public static WorkRecordParam Parse(WorkRecordAttempt workRecordAttempt)
    {
        return new(workRecordAttempt.WorkingDate,
                   workRecordAttempt.EmployeeNumber,
                   workRecordAttempt.EmployeeName,
                   workRecordAttempt.WorkingNumber,
                   workRecordAttempt.JigCode,
                   workRecordAttempt.ProcessFlow,
                   workRecordAttempt.Note,
                   workRecordAttempt.ManHour);
    }

    [Logging]
    public WorkRecord Convert()
        => WorkRecord.Create(
            WorkingDate,
            EmployeeNumber,
            EmployeeName,
            AchieveTrackService.ValueObjects.WorkingNumber.Create(WorkingNumber),
            JigCode,
            ProcessFlow,
            Note,
            AchieveTrackService.ValueObjects.ManHour.Create(ManHour));
}

public class TestAchievementRecordParamFactory
{
    [Logging]
    public static WorkRecordParam Create(DateTime? workingDate = default,
                                                uint employeeNumber = 4001u,
                                                string employeeName = "無人",
                                                string workingNumber = "23Z-1",
                                                string jigCode = "11A",
                                                string processFlow = "NC",
                                                string note = "特記事項",
                                                decimal manHour = 4)
    {
        workingDate ??= new DateTime(2023, 4, 1);
        return new WorkRecordParam(workingDate.Value,
                                   employeeNumber,
                                   employeeName,
                                   workingNumber,
                                   jigCode,
                                   processFlow,
                                   note,
                                   manHour);
    }
}