﻿using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using Wada.AchievementEntry.Models;
using Wada.AOP.Logging;

namespace Wada.AchievementEntry.ViewModels;

public class InvalidWorkNumberErrorCollectionViewModel : BindableBase, IDestructible, IValidationErrorCollectionViewModel
{
    private readonly IValidationError _model;

    public InvalidWorkNumberErrorCollectionViewModel(IValidationError validationResult)
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

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactivePropertySlim<string> Message { get; }

    public ReactivePropertySlim<string> WorkOrderId { get; }

    public ReactivePropertySlim<string> JigCode { get; }

    public ReactivePropertySlim<string> Note { get; }

    [Logging]
    internal static InvalidWorkNumberErrorCollectionViewModel Create(IValidationError validationResult)
        => new(validationResult);

    [Logging]
    internal static InvalidWorkNumberErrorCollectionViewModel Create(IValidationErrorCollectionViewModel validationError)
    {
        var _model = InvalidWorkOrderIdError.Create(validationError.WorkOrderId.Value, validationError.JigCode.Value, validationError.Note.Value);
        return Create(_model);
    }

    public void Destroy() => Disposables.Dispose();
}
