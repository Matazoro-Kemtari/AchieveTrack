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
    ReactivePropertySlim<string> WorkingNumber { get; }
    ReactivePropertySlim<string> JigCode { get; }
    ReactivePropertySlim<string> Note { get; }
}

public class InvalidWorkNumberResultCollectionViewModel : BindableBase, IDestructible, IValidationResultCollectionViewModel
{
    private readonly IValidationResultRequest _model;

    public InvalidWorkNumberResultCollectionViewModel(IValidationResultRequest validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);

        WorkingNumber = new ReactivePropertySlim<string>(_model.WorkingNumber.Value)
            .AddTo(Disposables);

        JigCode = new ReactivePropertySlim<string>(_model.JigCode)
            .AddTo(Disposables);

        Note = new ReactivePropertySlim<string>(_model.Note)
            .AddTo(Disposables);
    }

    internal static InvalidWorkNumberResultCollectionViewModel Create(IValidationResultRequest validationResult)
        => new(validationResult);

    internal static InvalidWorkNumberResultCollectionViewModel Create(IValidationResultCollectionViewModel validationResult)
    {
        var _model = InvalidWorkNumberResultRequest.Create(validationResult.WorkingNumber.Value, validationResult.JigCode.Value, validationResult.Note.Value);
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

public class DuplicateWorkDateEmployeeResultCollectionViewModel : BindableBase, IDestructible, IValidationResultCollectionViewModel
{
    private readonly IValidationResultRequest _model;

    public DuplicateWorkDateEmployeeResultCollectionViewModel(IValidationResultRequest validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);

        WorkingNumber = new ReactivePropertySlim<string>(_model.WorkingNumber.Value)
            .AddTo(Disposables);

        JigCode = new ReactivePropertySlim<string>(_model.JigCode)
            .AddTo(Disposables);

        Note = new ReactivePropertySlim<string>(_model.Note)
            .AddTo(Disposables);
    }

    internal static DuplicateWorkDateEmployeeResultCollectionViewModel Create(IValidationResultRequest validationResult)
        => new(validationResult);

    internal static DuplicateWorkDateEmployeeResultCollectionViewModel Create(IValidationResultCollectionViewModel validationResult)
    {
        var _model = DuplicateWorkDateEmployeeResultRequest.Create(validationResult.WorkingNumber.Value, validationResult.JigCode.Value, validationResult.Note.Value);
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

public class UnregisteredWorkNumberResultCollectionViewModel : BindableBase, IDestructible, IValidationResultCollectionViewModel
{
    private readonly IValidationResultRequest _model;

    public UnregisteredWorkNumberResultCollectionViewModel(IValidationResultRequest validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);

        WorkingNumber = new ReactivePropertySlim<string>(_model.WorkingNumber.Value)
            .AddTo(Disposables);

        JigCode = new ReactivePropertySlim<string>(_model.JigCode)
            .AddTo(Disposables);

        Note = new ReactivePropertySlim<string>(_model.Note)
            .AddTo(Disposables);
    }

    internal static UnregisteredWorkNumberResultCollectionViewModel Create(IValidationResultRequest validationResult)
        => new(validationResult);

    internal static UnregisteredWorkNumberResultCollectionViewModel Create(IValidationResultCollectionViewModel validationResult)
    {
        var _model = UnregisteredWorkNumberResultRequest.Create(validationResult.WorkingNumber.Value, validationResult.JigCode.Value, validationResult.Note.Value);
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

public class WorkDateExpiredResultCollectionViewModel : BindableBase, IDestructible, IValidationResultCollectionViewModel
{
    private readonly IValidationResultRequest _model;

    public WorkDateExpiredResultCollectionViewModel(IValidationResultRequest validationResult)
    {
        _model = validationResult;

        Message = new ReactivePropertySlim<string>(_model.Message)
            .AddTo(Disposables);

        WorkingNumber = new ReactivePropertySlim<string>(_model.WorkingNumber.Value)
            .AddTo(Disposables);

        JigCode = new ReactivePropertySlim<string>(_model.JigCode)
            .AddTo(Disposables);

        Note = new ReactivePropertySlim<string>(_model.Note)
            .AddTo(Disposables);
    }

    internal static WorkDateExpiredResultCollectionViewModel Create(IValidationResultRequest validationResult)
        => new(validationResult);

    internal static WorkDateExpiredResultCollectionViewModel Create(IValidationResultCollectionViewModel validationResult)
    {
        var _model = WorkDateExpiredResultRequest.Create(validationResult.WorkingNumber.Value, validationResult.JigCode.Value, validationResult.Note.Value);
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
