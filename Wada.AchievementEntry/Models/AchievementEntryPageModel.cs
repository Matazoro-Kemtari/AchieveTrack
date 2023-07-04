using Reactive.Bindings;
using Wada.AchievementEntry.ViewModels;
using Wada.AOP.Logging;

namespace Wada.AchievementEntry.Models;

class AchievementEntryPageModel
{
    [Logging]
    public void Clear()
    {
        AchievementCollections.Clear();
    }

    public ReactiveCollection<AchievementCollectionViewModel> AchievementCollections { get; } = new();
}
