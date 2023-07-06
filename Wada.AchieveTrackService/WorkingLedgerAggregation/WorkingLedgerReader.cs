using Wada.AchieveTrackService.ValueObjects;
using Wada.AOP.Logging;
using Wada.Data.OrderManagement.Models;

namespace Wada.AchieveTrackService.WorkingLedgerAggregation;

public class WorkingLedgerReader : IWorkingLedgerReader
{
    private readonly IWorkingLedgerRepository _workingLedgerRepository;

    public WorkingLedgerReader(IWorkingLedgerRepository workingLedgerRepository)
    {
        _workingLedgerRepository = workingLedgerRepository;
    }

    [Logging]
    public async Task<WorkingLedger> FindByWorkingNumberAsync(WorkingNumber workingNumber)
    {
        try
        {
            var workingLedger = await _workingLedgerRepository.FindByWorkingNumberAsync(workingNumber.Convert());
            return WorkingLedger.Parse(workingLedger);
        }
        catch (Data.OrderManagement.Models.WorkingLedgerAggregation.WorkingLedgerAggregationException ex)
        {
            throw new WorkingLedgerAggregationException(ex.Message, ex);
        }
    }
}
