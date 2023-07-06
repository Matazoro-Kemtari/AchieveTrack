using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;

namespace Wada.AchieveTrackService;

public interface IWorkingLedgerReader
{
    Task<WorkingLedger> FindByWorkingNumberAsync(WorkingNumber workingNumber);
}
