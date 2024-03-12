namespace Wada.AchieveTrackService.AchievementLedgerAggregation;

public record class AchievementLedger
{
    private AchievementLedger(uint id,
                              DateTime workingDate,
                              uint employeeNumber,
                              uint? departmentID,
                              IEnumerable<AchievementDetail> achievementDetails)
    {
        Id = id;
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        DepartmentID = departmentID;
        AchievementDetails = achievementDetails;
    }

    public static AchievementLedger Create(uint id,
                                           DateTime workingDate,
                                           uint employeeNumber,
                                           uint? departmentID,
                                           IEnumerable<AchievementDetail> achievementDetails)
        => new(id, workingDate, employeeNumber, departmentID, achievementDetails);

    public static AchievementLedger Reconstruct(uint id,
                                                DateTime workingDate,
                                                uint employeeNumber,
                                                uint? departmentID,
                                                IEnumerable<AchievementDetail> achievementDetails)
        => new(id, workingDate, employeeNumber, departmentID, achievementDetails);

    /// <summary>
    /// 実績ID
    /// </summary>
    public uint Id { get; }

    /// <summary>
    /// 作業日
    /// </summary>
    public DateTime WorkingDate { get; }

    /// <summary>
    /// 社員番号
    /// </summary>
    public uint EmployeeNumber { get; }

    /// <summary>
    /// 部署ID
    /// </summary>
    public uint? DepartmentID { get; }

    /// <summary>
    /// 実績台帳明細
    /// </summary>
    public IEnumerable<AchievementDetail> AchievementDetails { get; }
}

public class TestAchievementLedgerFacroty
{
    public static AchievementLedger Create(uint id = int.MaxValue,
                                           DateTime? workingDate = default,
                                           uint employeeNumber = 4001,
                                           uint? departmentID = default,
                                           IEnumerable<AchievementDetail>? achievementDetails = default)
    {
        workingDate ??= new DateTime(2023, 4, 1);
        achievementDetails ??= new List<AchievementDetail>
        {
            TestAchievementDetailFactory.Create(id: id),
        };
        return AchievementLedger.Reconstruct(
            id,
            workingDate.Value,
            employeeNumber,
            departmentID,
            achievementDetails);
    }
}