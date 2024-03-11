using Wada.AchieveTrackService.WorkRecordReader;

namespace Wada.ReadWorkRecordApplication;

public record class WorkRecordAttempt(DateTime WorkingDate,
                                      uint EmployeeNumber,
                                      string EmployeeName,
                                      string WorkingNumber,
                                      string? JigCode,
                                      string AchievementClassification,
                                      string? Note,
                                      decimal ManHour)
{
    public static WorkRecordAttempt Parse(WorkRecord workRecord)
        => new(workRecord.WorkingDate,
               workRecord.EmployeeNumber,
               workRecord.EmployeeName,
               workRecord.WorkingNumber.Value,
               workRecord.JigCode,
               workRecord.ProcessFlow,
               workRecord.Note,
               workRecord.ManHour.Value);
}
