using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ProcessFlowAggregation;
using Wada.DataBase.EFCore.OrderManagement;

namespace Wada.Data.OrderManagement;

public class ProcessFlowRepository(IConfiguration configuration) : IProcessFlowRepository
{
    private readonly IConfiguration _configuration = configuration;

    public async Task<ProcessFlow> FindByNameAsync(string name)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var processFlow = await dbContext.ProcessFlows.SingleAsync(x => x.Name == name);
            return ProcessFlow.Reconstruct((uint)processFlow.Id, processFlow.Name);
        }
        catch (InvalidOperationException ex)
        {
            throw new ProcessFlowNotFoundException(
                $"実績工程が見つかりません 実績工程: {name}", ex);
        }
    }
}
