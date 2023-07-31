namespace Wada.WriteWorkRecordApplication;

public record class AchievementParam(DateTime WorkingDate,
                                     uint EmployeeNumber,
                                     IEnumerable<AchievementDetailParam> AchievementDetails);

public class TestAchievementParamFactory
{
    public static AchievementParam Create(DateTime? workingDate = default,
                                          uint employeeNumber = 4001u,
                                          IEnumerable<AchievementDetailParam>? achievementDetails = default)
    {
        workingDate ??= new DateTime(2023, 4, 1);
        achievementDetails ??= new List<AchievementDetailParam>
        {
            TestAchievementDetailParamFactory.Create(),
        };
        return new AchievementParam(workingDate.Value, employeeNumber, achievementDetails);
    }
}
public record class AchievementDetailParam(string WorkingNumber,
                                           decimal ManHour);

public class TestAchievementDetailParamFactory
{
    public static AchievementDetailParam Create(string workingNumber = "22Z-1",
                                                decimal manHour = 10m)
        => new(workingNumber, manHour);
}