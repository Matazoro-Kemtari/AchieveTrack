namespace Wada.AchieveTrackService.AchievementLedgerAggregation;

public record class AchievementLedger
{
    private AchievementLedger(uint achievementId,
                              DateTime workingDate,
                              uint employeeNumber,
                              uint? departmentID,
                              IEnumerable<AchievementDetail> achievementDetails)
    {
        Id = Ulid.NewUlid();
        AchievementId = achievementId;
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        DepartmentID = departmentID;
        AchievementDetails = achievementDetails;
    }

    private AchievementLedger(string id,
                              uint achievementId,
                              DateTime workingDate,
                              uint employeeNumber,
                              uint? departmentID,
                              IEnumerable<AchievementDetail> achievementDetails)
    {
        Id = Ulid.Parse(id);
        AchievementId = achievementId;
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        DepartmentID = departmentID;
        AchievementDetails = achievementDetails;
    }

    public static AchievementLedger Create(uint achievementId,
                                           DateTime workingDate,
                                           uint employeeNumber,
                                           uint? departmentID,
                                           IEnumerable<AchievementDetail> achievementDetails)
        => new(achievementId, workingDate, employeeNumber, departmentID, achievementDetails);

    public static AchievementLedger Reconstruct(string id,
                                                uint achievementId,
                                                DateTime workingDate,
                                                uint employeeNumber,
                                                uint? departmentID,
                                                IEnumerable<AchievementDetail> achievementDetails)
        => new(id, achievementId, workingDate, employeeNumber, departmentID, achievementDetails);

    public Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger Convert()
        => Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger.Reconstruct(
            Id.ToString(),
            AchievementId,
            WorkingDate,
            EmployeeNumber,
            DepartmentID,
            (double?)AchievementDetails.Select(x => x.ManHour).Sum(),
            AchievementDetails.Select(x => x.Convert()));

    public static AchievementLedger Parse(Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger achievementLedger)
        => new(achievementLedger.Id.ToString(),
               achievementLedger.AchievementId,
               achievementLedger.WorkingDate,
               achievementLedger.EmployeeNumber,
               achievementLedger.DepartmentID,
               achievementLedger.AchievementDetails.Select(x => AchievementDetail.Parse(x)));

    /// <summary>
    /// EntityのID
    /// </summary>
    public Ulid Id { get; }

    /// <summary>
    /// 実績ID
    /// </summary>
    public uint AchievementId { get; }

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
    public static AchievementLedger Create(uint achievementId = int.MaxValue,
                                           DateTime? workingDate = default,
                                           uint employeeNumber = 4001,
                                           uint? departmentID = default,
                                           IEnumerable<AchievementDetail>? achievementDetails = default)
    {
        var id = Ulid.NewUlid();
        workingDate ??= new DateTime(2023, 4, 1);
        achievementDetails ??= new List<AchievementDetail>
        {
            TestAchievementDetailFactory.Create(),
        };
        return AchievementLedger.Reconstruct(
            id.ToString(),
            achievementId,
            workingDate.Value,
            employeeNumber,
            departmentID,
            achievementDetails);
    }
}