﻿using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AOP.Logging;

namespace Wada.DataSource.OrderManagement;

public class AchievementLedgerRepository : IAchievementLedgerRepository
{
    private readonly Data.OrderManagement.Models.IAchievementLedgerRepository _achievementLedgerRepository;

    public AchievementLedgerRepository(Data.OrderManagement.Models.IAchievementLedgerRepository achievementLedgerRepository)
    {
        _achievementLedgerRepository = achievementLedgerRepository;
    }

    [Logging]
    public int Add(AchievementLedger achievementLedger)
    {
        try
        {
            return _achievementLedgerRepository.Add(achievementLedger.Convert());
        }
        catch (Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedgerAggregationException ex)
        {
            throw new AchievementLedgerAggregationException(ex.Message, ex);
        }
    }

    [Logging]
    public async Task<AchievementLedger> FindByWorkingDateAndEmployeeNumberAsync(DateTime workingDate, uint employeeNumber)
    {
        try
        {
            var achievementLedger = await _achievementLedgerRepository.FindByWorkingDateAndEmployeeNumberAsync(workingDate, employeeNumber);
            return AchievementLedger.Parse(achievementLedger);
        }
        catch (Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedgerAggregationException ex)
        {
            throw new AchievementLedgerAggregationException(ex.Message, ex);
        }
    }

    [Logging]
    public async Task<AchievementLedger> MaxByAchievementIdAsync()
    {
        try
        {
            return AchievementLedger.Parse(
                await _achievementLedgerRepository.MaxByAchievementIdAsync());
        }
        catch (Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedgerAggregationException ex)
        {
            throw new AchievementLedgerAggregationException(ex.Message, ex);
        }
    }
}
