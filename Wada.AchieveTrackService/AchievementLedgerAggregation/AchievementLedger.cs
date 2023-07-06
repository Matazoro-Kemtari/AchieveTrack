namespace Wada.AchieveTrackService.AchievementLedgerAggregation;

public record class AchievementLedger
{
    private AchievementLedger(DateTime workingDate,
                              uint employeeNumber,
                              IEnumerable<AchievementDetail> achievementDetails)
    {
        Id = Ulid.NewUlid();
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        AchievementDetails = achievementDetails;
    }

    private AchievementLedger(uint? achievementId,
                              DateTime workingDate,
                              uint employeeNumber,
                              IEnumerable<AchievementDetail> achievementDetails)
    {
        Id = Ulid.NewUlid();
        AchievementId = achievementId;
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        AchievementDetails = achievementDetails;
    }

    public AchievementLedger(Ulid id,
                             uint? achievementId,
                             DateTime workingDate,
                             uint employeeNumber,
                             IEnumerable<AchievementDetail> achievementDetails)
    {
        Id = id;
        AchievementId = achievementId;
        WorkingDate = workingDate;
        EmployeeNumber = employeeNumber;
        AchievementDetails = achievementDetails;
    }

    public static AchievementLedger Create(DateTime workingDate,
                                           uint employeeNumber,
                                           IEnumerable<AchievementDetail> achievementDetails)
        => new(workingDate, employeeNumber, achievementDetails);

    public static AchievementLedger Reconstruct(uint? achievementId,
                                                DateTime workingDate,
                                                uint employeeNumber,
                                                IEnumerable<AchievementDetail> achievementDetails)
        => new(achievementId, workingDate, employeeNumber, achievementDetails);

    public static AchievementLedger Reconstruct(Ulid id,
                                                uint? achievementId,
                                                DateTime workingDate,
                                                uint employeeNumber,
                                                IEnumerable<AchievementDetail> achievementDetails)
        => new(id, achievementId, workingDate, employeeNumber, achievementDetails);

    /// <summary>
    /// AchievementLedgerのID
    /// </summary>
    public Ulid Id { get; }

    /// <summary>
    /// 実績ID
    /// 新規作成時はnull
    /// </summary>
    public uint? AchievementId { get; }

    /// <summary>
    /// 作業日
    /// </summary>
    public DateTime WorkingDate { get; }

    /// <summary>
    /// 社員番号
    /// </summary>
    public uint EmployeeNumber { get; }
    /// <summary>
    /// 実績台帳明細
    /// </summary>
    public IEnumerable<AchievementDetail> AchievementDetails { get; }
}

public class TestAchievementLedgerFacroty
{
    public static AchievementLedger Create(uint? achievementId = default,
                                           DateTime? workingDate = default,
                                           uint employeeNumber = 4001,
                                           IEnumerable<AchievementDetail>? achievementDetails = default)
    {
        var id = Ulid.NewUlid();
        workingDate ??= new DateTime(2023, 4, 1);
        achievementDetails ??= new List<AchievementDetail>
        {
            TestAchievementDetailFactory.Create(),
        };
        return AchievementLedger.Reconstruct(id, achievementId, workingDate.Value, employeeNumber, achievementDetails);
    }
}