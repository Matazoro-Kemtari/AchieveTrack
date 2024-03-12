namespace Wada.AchieveTrackService.AchievementLedgerAggregation;

public record class AchievementDetail
{
    private AchievementDetail(uint id, uint ownCompanyNumber, uint achievementProcessId, decimal? manHour)
    {
        Id = id;
        OwnCompanyNumber = ownCompanyNumber;
        AchievementProcessId = achievementProcessId;
        ManHour = manHour;
    }

    public static AchievementDetail Create(uint id,
                                           uint ownCompanyNumber,
                                           uint achievementProcessId,
                                           decimal? manHour)
        => new(id, ownCompanyNumber, achievementProcessId, manHour);

    public static AchievementDetail Reconstruct(uint id,
                                                uint ownCompanyNumber,
                                                uint achievementProcessId,
                                                decimal? manHour)
        => new(id, ownCompanyNumber, achievementProcessId, manHour);

    /// <summary>
    /// 実績ID
    /// </summary>
    public uint Id { get; }

    /// <summary>
    /// 自社NO
    /// </summary>
    public uint OwnCompanyNumber { get; }

    /// <summary>
    /// 実績工程ID
    /// </summary>
    public uint AchievementProcessId { get; }

    /// <summary>
    /// 工数
    /// </summary>
    public decimal? ManHour { get; }
}

public class TestAchievementDetailFactory
{
    public static AchievementDetail Create(uint id = int.MaxValue,
                                           uint ownCompanyNumber = int.MaxValue,
                                           uint achievementProcessId = int.MaxValue,
                                           decimal? manHour = 10m)
        => AchievementDetail.Create(id, ownCompanyNumber, achievementProcessId, manHour);
}
