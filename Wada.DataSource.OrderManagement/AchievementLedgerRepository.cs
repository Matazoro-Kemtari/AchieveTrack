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
    public async Task<int> AddAsync(AchievementLedger achievementLedger)
    {
        try
        {
            return await _achievementLedgerRepository.AddAsync(achievementLedger.Convert());
        }
        catch (Exception)
        {

            throw;
        }
        throw new NotImplementedException();
    }

    [Logging]
    public async Task<IEnumerable<AchievementLedger>> FindAllAsync()
    {
        var results = await _achievementLedgerRepository.FindAllAsync();
        return results.Select(x => AchievementLedger.Parse(x));
    }
}
