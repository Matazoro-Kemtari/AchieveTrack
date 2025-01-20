using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkOrderAggregation;

public record class WorkOrder
{
    private WorkOrder(uint ownCompanyNumber,
                          WorkOrderId workOrderId,
                          DateTime? completionDate)
    {
        OwnCompanyNumber = ownCompanyNumber;
        WorkOrderId = workOrderId;
        CompletionDate = completionDate;
    }

    // ファクトリメソッド
    // 受注管理にデータを追加する必要性が出たら追加する

    /// <summary>
    /// インフラ層専用
    /// </summary>
    /// <param name="ownCompanyNumber"></param>
    /// <param name="workOrderId"></param>
    /// <returns></returns>
    public static WorkOrder Reconstruct(uint ownCompanyNumber,
                                            WorkOrderId workOrderId,
                                            DateTime? completionDate)
        => new(
            ownCompanyNumber,
            workOrderId,
            completionDate);

    /// <summary>
    /// 自社NO
    /// </summary>
    public uint OwnCompanyNumber { get; }

    /// <summary>
    /// 作業番号
    /// </summary>
    public WorkOrderId WorkOrderId { get; }

    /// <summary>
    /// 完成日
    /// </summary>
    public DateTime? CompletionDate { get; }
}

public class TestWorkOrderFactory
{
    public static WorkOrder Create(uint ownCompanyNumber = 2002010040u,
                                       WorkOrderId? workOrderId = default,
                                       DateTime? completionDate = default)
    {
        workOrderId ??= TestWorkOrderIdFactory.Create();
        return WorkOrder.Reconstruct(ownCompanyNumber,
                                         workOrderId,
                                         completionDate);
    }
}
