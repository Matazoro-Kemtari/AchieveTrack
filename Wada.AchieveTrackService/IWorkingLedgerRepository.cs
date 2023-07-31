using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;

namespace Wada.AchieveTrackService;

public interface IWorkingLedgerRepository
{
    /// <summary>
    /// 作業番号で作業台帳を検索する
    /// </summary>
    /// <param name="workingNumber"></param>
    /// <returns></returns>
    Task<WorkingLedger> FindByWorkingNumberAsync(WorkingNumber workingNumber);
}
