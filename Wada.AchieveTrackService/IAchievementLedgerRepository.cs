using Wada.AchieveTrackService.AchievementLedgerAggregation;

namespace Wada.AchieveTrackService;

public interface IAchievementLedgerRepository
{
    Task<int> AddAsync(AchievementLedger achievementLedger);
}
