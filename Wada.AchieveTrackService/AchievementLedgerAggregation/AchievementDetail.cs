namespace Wada.AchieveTrackService.AchievementLedgerAggregation;

public record class AchievementDetail
{
    private AchievementDetail(uint achievementId, uint ownCompanyNumber, uint achievementProcessId, decimal? manHour)
    {
        Id = Ulid.NewUlid();
        AchievementId = achievementId;
        OwnCompanyNumber = ownCompanyNumber;
        AchievementProcessId = achievementProcessId;
        ManHour = manHour;
    }

    private AchievementDetail(string id, uint achievementId, uint ownCompanyNumber, uint achievementProcessId, decimal? manHour)
    {
        Id = Ulid.Parse(id);
        AchievementId = achievementId;
        OwnCompanyNumber = ownCompanyNumber;
        AchievementProcessId = achievementProcessId;
        ManHour = manHour;
    }

    public static AchievementDetail Create(uint achievementId,
                                           uint ownCompanyNumber,
                                           uint achievementProcessId,
                                           decimal? manHour)
        => new(achievementId, ownCompanyNumber, achievementProcessId, manHour);

    public static AchievementDetail Reconstruct(string id,
                                                uint achievementId,
                                                uint ownCompanyNumber,
                                                uint achievementProcessId,
                                                decimal? manHour)
        => new(id, achievementId, ownCompanyNumber, achievementProcessId, manHour);

    internal Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementDetail Convert()
        => Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementDetail.Reconstruct(
            Id.ToString(), AchievementId, OwnCompanyNumber, AchievementProcessId, (double?)ManHour,
            (double?)ManHour);

    internal static AchievementDetail Parse(Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementDetail achievementDetail)
        => new(achievementDetail.Id.ToString(),
               achievementDetail.AchievementLedgerId,
               achievementDetail.OwnCompanyNumber,
               achievementDetail.AchievementProcessId,
               (decimal?)achievementDetail.ActualManHour);


    /// <summary>
    /// EntityのID
    /// </summary>
    public Ulid Id { get; }

    /// <summary>
    /// 実績ID
    /// </summary>
    public uint AchievementId { get; }

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
    public static AchievementDetail Create(uint achievementId = int.MaxValue,
                                           uint ownCompanyNumber = int.MaxValue,
                                           uint achievementProcessId = int.MaxValue,
                                           decimal? manHour = 10m)
        => AchievementDetail.Create(achievementId, ownCompanyNumber, achievementProcessId, manHour);
}
