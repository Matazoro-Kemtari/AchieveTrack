using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AOP.Logging;
using Wada.DataBase.EFCore.OrderManagement;

namespace Wada.Data.OrderManagement;

public class AchievementLedgerRepository(IConfiguration configuration) : IAchievementLedgerRepository
{
    private readonly IConfiguration _configuration = configuration;

    [Logging]
    public int Add(AchievementLedger achievementLedger)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var additionalItem = new DataBase.EFCore.OrderManagement.Entities.AchievementLedger()
            {
                Id = (int)achievementLedger.Id,
                WorkingDate = achievementLedger.WorkingDate,
                EmployeeNumber = (int)achievementLedger.EmployeeNumber,
                DepartmentID = (int?)achievementLedger.DepartmentID,
                ActualWorkTime = (double?)achievementLedger.AchievementDetails.Sum(x => x.ManHour),
            };
            _ = dbContext.AchievementLedgers.Add(additionalItem);
            dbContext.AchievementDetails.AddRange(
                achievementLedger.AchievementDetails.Select(
                    x => new DataBase.EFCore.OrderManagement.Entities.AchievementDetail()
                    {
                        AchievementLedgerId = (int)x.Id,
                        OwnCompanyNumber = (int)x.OwnCompanyNumber,
                        AchievementProcessId = (int)x.AchievementProcessId,
                        TargetManHour = (double?)x.ManHour,
                        ActualManHour = (double?)x.ManHour,
                    }));
            return dbContext.SaveChanges();
        }
        catch (Exception ex) when (ex is OperationCanceledException or DbUpdateException)
        {
            throw new AchievementLedgerAggregationException(
                $"実績台帳に登録できませんでした レコード: {achievementLedger}", ex);
        }
    }

    [Logging]
    public async Task<AchievementLedger> FindByWorkingDateAndEmployeeNumberAsync(DateTime workingDate, uint employeeNumber)
    {
        using var dbContext = new OrderManagementContext(_configuration);
        try
        {
            var achievementLedger = await dbContext.AchievementLedgers.Where(x => x.WorkingDate == workingDate)
                                                                      .Where(x => x.EmployeeNumber == employeeNumber)
                                                                      .Include(x => x.AchievementDetails)
                                                                      .FirstAsync();
            return ConvertDomainEntity(achievementLedger);
        }
        catch (InvalidOperationException ex)
        {
            throw new AchievementLedgerAggregationException(
                "作業日と社員番号を確認してください " +
                "実績台帳に登録されていません " +
                $"作業日: {workingDate}, " +
                $"社員番号: {employeeNumber}", ex);
        }
    }

    [Logging]
    public async Task<AchievementLedger> MaxByAchievementIdAsync()
    {
        using var dbContext = new OrderManagementContext(_configuration);
        int maxId;
        try
        {
            maxId = await dbContext.AchievementLedgers.MaxAsync(x => x.Id);
        }
        catch (InvalidOperationException)
        {
            throw new AchievementLedgerAggregationException("実績台帳が登録されていません");
        }
        var achievementLedger = dbContext
            .AchievementLedgers.Where(x => x.Id == maxId)
                               .Include(x => x.AchievementDetails)
                               .Single();
        return ConvertDomainEntity(achievementLedger);
    }

    private static AchievementLedger ConvertDomainEntity(DataBase.EFCore.OrderManagement.Entities.AchievementLedger achievementLedger)
        => AchievementLedger.Reconstruct(
            (uint)achievementLedger.Id,
            achievementLedger.WorkingDate,
            (uint)achievementLedger.EmployeeNumber,
            (uint?)achievementLedger.DepartmentID,
            achievementLedger.AchievementDetails.Select(
                x => AchievementDetail.Reconstruct(
                    (uint)x.AchievementLedgerId,
                    (uint)x.OwnCompanyNumber,
                    (uint)x.AchievementProcessId,
                    (decimal?)x.ActualManHour)));

    [Logging]
    public IEnumerable<AchievementLedger> FindAll()
    {
        using var dbContext = new OrderManagementContext(_configuration);
        return dbContext.AchievementLedgers.Include(x => x.AchievementDetails)
                                           .Select(x => ConvertDomainEntity(x))
                                           .ToList();
    }
}
