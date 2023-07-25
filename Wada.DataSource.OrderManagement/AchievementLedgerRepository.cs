using Wada.AchieveTrackService;
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
    public async Task<IEnumerable<AchievementLedger>> FindAllAsync()
    {
        var results = await _achievementLedgerRepository.FindAllAsync();
        return results.Select(x => AchievementLedger.Parse(x));
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
