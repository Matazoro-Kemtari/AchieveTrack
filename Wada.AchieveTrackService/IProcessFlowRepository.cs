using Wada.AchieveTrackService.ProcessFlowAggregation;

namespace Wada.AchieveTrackService;

public interface IProcessFlowRepository
{
    Task<ProcessFlow> FindByNameAsync(string name);
}
