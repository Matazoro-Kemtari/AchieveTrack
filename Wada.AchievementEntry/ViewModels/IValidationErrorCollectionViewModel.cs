using Prism.Navigation;
using Reactive.Bindings;

namespace Wada.AchievementEntry.ViewModels;

public interface IValidationErrorCollectionViewModel : IDestructible
{
    ReactivePropertySlim<string> Message { get; }
    ReactivePropertySlim<string> WorkingNumber { get; }
    ReactivePropertySlim<string> JigCode { get; }
    ReactivePropertySlim<string> Note { get; }
}
