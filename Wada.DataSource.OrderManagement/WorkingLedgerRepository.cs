using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;
using Wada.AOP.Logging;

namespace Wada.DataSource.OrderManagement;

public class WorkingLedgerRepository : IWorkingLedgerRepository
{
    private readonly Data.OrderManagement.Models.IWorkingLedgerRepository _workingLedgerRepository;

    public WorkingLedgerRepository(Data.OrderManagement.Models.IWorkingLedgerRepository workingLedgerRepository)
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
