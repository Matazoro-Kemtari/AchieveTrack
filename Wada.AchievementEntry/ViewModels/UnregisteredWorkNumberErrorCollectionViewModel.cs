using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using Wada.AchievementEntry.Models;
using Wada.AOP.Logging;

namespace Wada.AchievementEntry.ViewModels;

public class UnregisteredWorkNumberErrorCollectionViewModel : BindableBase, IDestructible, IValidationErrorCollectionViewModel
{
    private readonly IValidationError _model;

    public UnregisteredWorkNumberErrorCollectionViewModel(IValidationError validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);

        WorkingNumber = new ReactivePropertySlim<string>(_model.WorkingNumber)
            .AddTo(Disposables);

        JigCode = new ReactivePropertySlim<string>(_model.JigCode)
            .AddTo(Disposables);

        Note = new ReactivePropertySlim<string>(_model.Note)
            .AddTo(Disposables);
    }

    [Logging]
    internal static UnregisteredWorkNumberErrorCollectionViewModel Create(IValidationError validationResult)
        => new(validationResult);

    [Logging]
    internal static UnregisteredWorkNumberErrorCollectionViewModel Create(IValidationErrorCollectionViewModel validationResult)
    {
        var _model = UnregisteredWorkNumberError.Create(validationResult.WorkingNumber.Value, validationResult.JigCode.Value, validationResult.Note.Value);
        return Create(_model);
    }

    public void Destroy() => Disposables.Dispose();

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactivePropertySlim<string> Message { get; }

    public ReactivePropertySlim<string> WorkingNumber { get; }

    public ReactivePropertySlim<string> JigCode { get; }

    public ReactivePropertySlim<string> Note { get; }
}
