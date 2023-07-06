using Wada.AchieveTrackService.ValueObjects;
using Wada.Data.OrderManagement.Models.WorkingLedgerAggregation;

namespace Wada.AchieveTrackService.WorkingLedgerAggregation;

public record class WorkingLedger
{
    private WorkingLedger(uint ownCompanyNumber,
                          WorkingNumber workingNumber)
    {
        OwnCompanyNumber = ownCompanyNumber;
        WorkingNumber = workingNumber;
    }

    // ファクトリメソッド
    // 受注管理にデータを追加する必要性が出たら追加する

    /// <summary>
    /// インフラ層専用
    /// </summary>
    /// <param name="ownCompanyNumber"></param>
    /// <param name="workingNumber"></param>
    /// <returns></returns>
    public static WorkingLedger Reconstruct(uint ownCompanyNumber,
                                            WorkingNumber workingNumber)
        => new(
            ownCompanyNumber,
            workingNumber);

    /// <summary>
    /// 自社NO
    /// </summary>
    public uint OwnCompanyNumber { get; }

    /// <summary>
    /// 作業番号
    /// </summary>
    public WorkingNumber WorkingNumber { get; }
}

public class TestWorkingLedgerFactory
{
    public static WorkingLedger Create(uint ownCompanyNumber = 2002010040u,
                                       WorkingNumber? workingNumber = default)
    {
        workingNumber ??= TestWorkingNumberFactory.Create();
        return WorkingLedger.Reconstruct(ownCompanyNumber,
                                         workingNumber);
    }
}
