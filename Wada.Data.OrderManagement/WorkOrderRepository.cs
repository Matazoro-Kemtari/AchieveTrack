using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkOrderAggregation;
using Wada.AOP.Logging;
using Wada.DataBase.EFCore.OrderManagement;

namespace Wada.Data.OrderManagement;

public class WorkOrderRepository(IConfiguration configuration) : IWorkOrderRepository
{
    private readonly IConfiguration _configuration = configuration;

    [Logging]
    public async Task<WorkOrder> FindByWorkOrderIdAsync(WorkOrderId workOrderId)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var workOrder = await dbContext.WorkOrders.SingleAsync(x => x.WorkOrderId == workOrderId.Value);
            return WorkOrder.Reconstruct((uint)workOrder.OwnCompanyNumber,
                                             WorkOrderId.Create(workOrder.WorkOrderId),
                                             workOrder.CompletionDate);
        }
        catch (InvalidOperationException ex)
        {
            throw new WorkOrderNotFoundException(
                $"作業番号を確認してください 受注管理に登録されていません 作業番号: {workOrderId}", ex);
        }
    }
}
