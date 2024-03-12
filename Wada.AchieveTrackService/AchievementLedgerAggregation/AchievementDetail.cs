namespace Wada.AchieveTrackService.AchievementLedgerAggregation;

public record class AchievementDetail
{
    private AchievementDetail(uint id, uint ownCompanyNumber, uint processFlowId, decimal? manHour)
    {
        Id = id;
        OwnCompanyNumber = ownCompanyNumber;
        ProcessFlowId = processFlowId;
        ManHour = manHour;
    }

    public static AchievementDetail Create(uint id,
                                           uint ownCompanyNumber,
                                           uint processFlowId,
                                           decimal? manHour)
        => new(id, ownCompanyNumber, processFlowId, manHour);

    public static AchievementDetail Reconstruct(uint id,
                                                uint ownCompanyNumber,
                                                uint processFlowId,
                                                decimal? manHour)
        => new(id, ownCompanyNumber, processFlowId, manHour);

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
    public uint ProcessFlowId { get; }

    /// <summary>
    /// 工数
    /// </summary>
    public decimal? ManHour { get; }
}

public class TestAchievementDetailFactory
{
    public static AchievementDetail Create(uint id = int.MaxValue,
                                           uint ownCompanyNumber = int.MaxValue,
                                           uint processFlowId = int.MaxValue,
                                           decimal? manHour = 10m)
        => AchievementDetail.Create(id, ownCompanyNumber, processFlowId, manHour);
}
