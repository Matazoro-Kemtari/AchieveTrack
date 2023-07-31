using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Wada.AchievementEntry.Views;

namespace Wada.AchievementEntry
{
    public class AchievementEntryModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager?.RequestNavigate("ContentRegion", nameof(AchievementEntryPage));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AchievementEntryPage>();
        }
    }
}