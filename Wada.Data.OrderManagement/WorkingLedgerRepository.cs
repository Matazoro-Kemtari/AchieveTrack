using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;
using Wada.AOP.Logging;
using Wada.DataBase.EFCore.OrderManagement;

namespace Wada.Data.OrderManagement;

public class WorkingLedgerRepository(IConfiguration configuration) : IWorkingLedgerRepository
{
    private readonly IConfiguration _configuration = configuration;

    [Logging]
    public async Task<WorkingLedger> FindByWorkingNumberAsync(WorkingNumber workingNumber)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var workingLedger = await dbContext.WorkingLedgers.SingleAsync(x => x.WorkingNumber == workingNumber.Value);
            return WorkingLedger.Reconstruct((uint)workingLedger.OwnCompanyNumber,
                                             WorkingNumber.Create(workingLedger.WorkingNumber),
                                             workingLedger.CompletionDate);
        }
        catch (InvalidOperationException ex)
        {
            throw new WorkingLedgerNotFoundException(
                $"作業番号を確認してください 受注管理に登録されていません 作業番号: {workingNumber}", ex);
        }
    }
}
