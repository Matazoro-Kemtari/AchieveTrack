using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.DesignManagementAggregation;
using Wada.AOP.Logging;
using Wada.DataBase.EFCore.OrderManagement;

namespace Wada.Data.OrderManagement;

public class DesignManagementRepository(IConfiguration configuration) : IDesignManagementRepository
{
    private readonly IConfiguration _configuration = configuration;

    [Logging]
    public int Add(DesignManagement designManagement)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var item = new DataBase.EFCore.OrderManagement.Entities.DesignManagement()
            {
                OwnCompanyNumber = (int)designManagement.OwnCompanyNumber,
                StartDate = designManagement.StartDate,
                DesignLead = designManagement.DesignLead,
            };
            _ = dbContext.DesignManagements.Add(item);
            return dbContext.SaveChanges();
        }
        catch (Exception ex) when (ex is OperationCanceledException or DbUpdateException)
        {
            var message = $"設計管理に登録できませんでした 自社NOを確認してください " +
                $"自社NO: {designManagement.OwnCompanyNumber}";
            throw new DesignManagementAggregationException(message, ex);
        }
    }

    [Logging]
    public IEnumerable<DesignManagement> FindAll()
    {
        using var dbContext = new OrderManagementContext(_configuration);
        return dbContext.DesignManagements.Select(x => ConvertDomainEntity(x))
                                          .ToList();
    }

    [Logging]
    public DesignManagement FindByOwnCompanyNumber(uint ownCompanyNumber)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var item = dbContext.DesignManagements.First(
                x => x.OwnCompanyNumber == ownCompanyNumber);
            return ConvertDomainEntity(item);
        }
        catch (InvalidOperationException ex)
        {
            throw new DesignManagementNotFoundException(
                $"設計管理に該当がありません 自社NOを確認してください 自社NO: {ownCompanyNumber}", ex);
        }
    }

    [Logging]
    public async Task<DesignManagement> FindByOwnCompanyNumberAsync(uint ownCompanyNumber)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var item = await dbContext.DesignManagements.FirstAsync(
                x => x.OwnCompanyNumber == ownCompanyNumber);
            return ConvertDomainEntity(item);
        }
        catch (InvalidOperationException ex)
        {
            throw new DesignManagementNotFoundException(
                $"設計管理に該当がありません 自社NOを確認してください 自社NO: {ownCompanyNumber}", ex);
        }
    }

    [Logging]
    private static DesignManagement ConvertDomainEntity(DataBase.EFCore.OrderManagement.Entities.DesignManagement item)
        => DesignManagement.Reconstruct((uint)item.OwnCompanyNumber, item.StartDate, item.DesignLead);
}
