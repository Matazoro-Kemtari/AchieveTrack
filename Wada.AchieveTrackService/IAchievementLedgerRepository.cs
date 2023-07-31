using Wada.AchieveTrackService.AchievementLedgerAggregation;

namespace Wada.AchieveTrackService;

public interface IAchievementLedgerRepository
{
    /// <summary>
    /// 実績台帳に追加する
    /// </summary>
    /// <param name="achievementLedger"></param>
    /// <returns>追加件数</returns>
    int Add(AchievementLedger achievementLedger);

    /// <summary>
    /// 実績台帳を作業日と社員番号で検索する
    /// </summary>
    /// <param name="workingDate"></param>
    /// <param name="employeeNumber"></param>
    /// <returns></returns>
    Task<AchievementLedger> FindByWorkingDateAndEmployeeNumberAsync(DateTime workingDate, uint employeeNumber);

    /// <summary>
    /// 実績台帳の実績IDの最大値を検索する
    /// </summary>
    /// <returns></returns>
    Task<AchievementLedger> MaxByAchievementIdAsync();
}
