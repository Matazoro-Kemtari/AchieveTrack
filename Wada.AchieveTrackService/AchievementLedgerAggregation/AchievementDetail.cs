namespace Wada.AchieveTrackService.AchievementLedgerAggregation;

public record class AchievementDetail
{
    private AchievementDetail(uint ownCompanyNumber, decimal? manHour)
    {
        OwnCompanyNumber = ownCompanyNumber;
        ManHour = manHour;
    }

    public static AchievementDetail Create(uint ownCompanyNumber,
                                           decimal? manHour)
        => new(ownCompanyNumber, manHour);

    public static AchievementDetail Reconstruct(uint ownCompanyNumber,
                                                decimal? manHour)
        => new(ownCompanyNumber, manHour);

    /// <summary>
    /// 自社NO
    /// </summary>
    public uint OwnCompanyNumber { get; }

    /// <summary>
    /// 工数
    /// </summary>
    public decimal? ManHour { get; }
}

public class TestAchievementDetailFactory
{
    public static AchievementDetail Create(uint ownCompanyNumber = int.MaxValue,
                                           decimal? manHour = 10m)
        => AchievementDetail.Reconstruct(ownCompanyNumber, manHour);
}
