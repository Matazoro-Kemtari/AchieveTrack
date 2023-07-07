using Wada.AchieveTrackService.AchievementLedgerAggregation;

namespace Wada.AchieveTrackService;

public interface IAchievementLedgerRepository
{
    /// <summary>
    /// 実績台帳に追加する
    /// </summary>
    /// <param name="achievementLedger"></param>
    /// <returns>追加件数</returns>
    Task<int> AddAsync(AchievementLedger achievementLedger);

    /// <summary>
    /// 実績台帳を全て取得する
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<AchievementLedger>> FindAllAsync();
}
