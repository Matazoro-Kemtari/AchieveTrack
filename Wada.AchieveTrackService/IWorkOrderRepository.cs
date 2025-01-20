using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkOrderAggregation;

namespace Wada.AchieveTrackService;

public interface IWorkOrderRepository
{
    /// <summary>
    /// 作業番号で作業台帳を検索する
    /// </summary>
    /// <param name="workOrderId"></param>
    /// <returns></returns>
    Task<WorkOrder> FindByWorkOrderIdAsync(WorkOrderId workOrderId);
}
