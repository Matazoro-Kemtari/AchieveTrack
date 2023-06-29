using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using Wada.AchievementEntry.Models;

namespace Wada.AchievementEntry.ViewModels;

public interface IValidationResultCollectionViewModel : IDestructible
{
    ReactivePropertySlim<string> Message { get; }
}

public class InvalidWorkNumberResultCollectionViewModel : BindableBase, IDestructible, IValidationResultCollectionViewModel
{
    private readonly IValidationResultRequest _model;

    public InvalidWorkNumberResultCollectionViewModel(IValidationResultRequest validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);
    }

    internal static InvalidWorkNumberResultCollectionViewModel Create(IValidationResultRequest validationResult)
        => new(validationResult);

    public void Destroy() => Disposables.Dispose();

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactivePropertySlim<string> Message { get; }
}

public class DuplicateWorkDateEmployeeResultCollectionViewModel : BindableBase, IDestructible, IValidationResultCollectionViewModel
{
    private readonly IValidationResultRequest _model;

    public DuplicateWorkDateEmployeeResultCollectionViewModel(IValidationResultRequest validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);
    }

    internal static DuplicateWorkDateEmployeeResultCollectionViewModel Create(IValidationResultRequest validationResult)
        => new(validationResult);

    public void Destroy() => Disposables.Dispose();

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactivePropertySlim<string> Message { get; }
}

public class UnregisteredWorkNumberResultCollectionViewModel : BindableBase, IDestructible, IValidationResultCollectionViewModel
{
    private readonly IValidationResultRequest _model;

    public UnregisteredWorkNumberResultCollectionViewModel(IValidationResultRequest validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);
    }

    internal static UnregisteredWorkNumberResultCollectionViewModel Create(IValidationResultRequest validationResult)
        => new(validationResult);

    public void Destroy() => Disposables.Dispose();

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactivePropertySlim<string> Message { get; }
}

public class WorkDateExpiredResultCollectionViewModel : BindableBase, IDestructible, IValidationResultCollectionViewModel
{
    private readonly IValidationResultRequest _model;

    public WorkDateExpiredResultCollectionViewModel(IValidationResultRequest validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);
    }

    internal static WorkDateExpiredResultCollectionViewModel Create(IValidationResultRequest validationResult)
        => new(validationResult);

    public void Destroy() => Disposables.Dispose();

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactivePropertySlim<string> Message { get; }
}
