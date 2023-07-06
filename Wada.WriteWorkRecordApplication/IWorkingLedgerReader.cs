using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;

namespace Wada.WriteWorkRecordApplication;

public interface IWorkingLedgerReader
{
    Task<WorkingLedger> FindByWorkingNumberAsync(WorkingNumber workingNumber);
}
