using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using Wada.AchievementEntry.Models;
using Wada.AOP.Logging;

namespace Wada.AchievementEntry.ViewModels;

public class AchievementCollectionViewModel : BindableBase, IDestructible
{
    private readonly AchievementCollectionModel _model;

    private AchievementCollectionViewModel()
    {

        _model = new();

        CheckedItem = _model.CheckedItem.ToReactivePropertyAsSynchronized(x => x.Value)
                                        .AddTo(Disposables);

        AchievementDate = _model.AchievementDate.ToReactivePropertyAsSynchronized(x => x.Value)
                                                .AddTo(Disposables);

        EmployeeNumber = _model.EmployeeNumber.ToReactivePropertyAsSynchronized(x => x.Value)
                                              .AddTo(Disposables);

        EmployeeName = _model.EmployeeName.ToReactivePropertyAsSynchronized(x => x.Value)
                                          .AddTo(Disposables);

        ValidationResults = _model.ValidationResults.ToReadOnlyReactiveCollection()
                                                    .AddTo(Disposables);
    }

    [Logging]
    internal static AchievementCollectionViewModel Create(AchievementCollectionModel achievementCollectionModel)
    {
        var vm = new AchievementCollectionViewModel();
        vm.Apply(achievementCollectionModel);
        return vm;
    }

    [Logging]
    private void Apply(AchievementCollectionModel achievementCollectionModel)
    {
        _model.CheckedItem.Value = achievementCollectionModel.CheckedItem.Value;
        _model.AchievementDate.Value = achievementCollectionModel.AchievementDate.Value;
        _model.EmployeeNumber.Value = achievementCollectionModel.EmployeeNumber.Value;
        _model.EmployeeName.Value = achievementCollectionModel.EmployeeName.Value;

        var vmCreater = new Dictionary<Type, Func<IValidationErrorCollectionViewModel, IValidationErrorCollectionViewModel>>
        {
            { typeof(InvalidWorkNumberErrorCollectionViewModel), InvalidWorkNumberErrorCollectionViewModel.Create },
            { typeof(DuplicateWorkDateEmployeeErrorCollectionViewModel), DuplicateWorkDateEmployeeErrorCollectionViewModel.Create },
            { typeof(UnregisteredWorkOrderIdErrorCollectionViewModel), UnregisteredWorkOrderIdErrorCollectionViewModel.Create },
            { typeof(WorkDateExpiredErrorCollectionViewModel), WorkDateExpiredErrorCollectionViewModel.Create },
        };

        _model.ValidationResults.AddRange(achievementCollectionModel.ValidationResults.Select(x => vmCreater[x.GetType()](x)));

        HasErrors.Value = _model.ValidationResults.Any();
        HasErrorsWithOutDesignManagement.Value =
            _model.ValidationResults.Any(
                x => x.GetType() != typeof(UnregisteredWorkOrderIdErrorCollectionViewModel));
    }

    public void Destroy() => Disposables.Dispose();

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactiveProperty<bool> CheckedItem { get; }

    public ReactiveProperty<DateTime> AchievementDate { get; }

    public ReactiveProperty<uint> EmployeeNumber { get; }

    public ReactiveProperty<string?> EmployeeName { get; }

    public ReadOnlyReactiveCollection<IValidationErrorCollectionViewModel> ValidationResults { get; }

    public ReactivePropertySlim<bool> HasErrors { get; } = new();

    public ReactivePropertySlim<bool> HasErrorsWithOutDesignManagement { get; } = new();
}
