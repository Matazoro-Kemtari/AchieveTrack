using Reactive.Bindings;
using Wada.AchievementEntry.ViewModels;
using Wada.AOP.Logging;
using Wada.ReadWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

class AchievementEntryPageModel
{
    [Logging]
    public void Clear()
    {
        AchievementCollections.Clear();
    }

    public ReactiveCollection<AchievementCollectionViewModel> AchievementCollections { get; } = new();
    
    public ReactiveCollection<WorkRecordAttempt> WorkRecords { get; } = new();
}
