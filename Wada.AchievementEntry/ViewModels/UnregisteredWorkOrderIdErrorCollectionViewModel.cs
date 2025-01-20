using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using Wada.AchievementEntry.Models;
using Wada.AOP.Logging;

namespace Wada.AchievementEntry.ViewModels;

public class UnregisteredWorkOrderIdErrorCollectionViewModel : BindableBase, IDestructible, IValidationErrorCollectionViewModel
{
    private readonly IValidationError _model;

    public UnregisteredWorkOrderIdErrorCollectionViewModel(IValidationError validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);

        WorkOrderId = new ReactivePropertySlim<string>(_model.WorkOrderId)
            .AddTo(Disposables);

        JigCode = new ReactivePropertySlim<string>(_model.JigCode)
            .AddTo(Disposables);

        Note = new ReactivePropertySlim<string>(_model.Note)
            .AddTo(Disposables);
    }

    [Logging]
    internal static UnregisteredWorkOrderIdErrorCollectionViewModel Create(IValidationError validationResult)
        => new(validationResult);

    [Logging]
    internal static UnregisteredWorkOrderIdErrorCollectionViewModel Create(IValidationErrorCollectionViewModel validationResult)
    {
        var _model = UnregisteredWorkOrderIdError.Create(validationResult.WorkOrderId.Value, validationResult.JigCode.Value, validationResult.Note.Value);
        return Create(_model);
    }

    public void Destroy() => Disposables.Dispose();

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactivePropertySlim<string> Message { get; }

    public ReactivePropertySlim<string> WorkOrderId { get; }

    public ReactivePropertySlim<string> JigCode { get; }

    public ReactivePropertySlim<string> Note { get; }
}
