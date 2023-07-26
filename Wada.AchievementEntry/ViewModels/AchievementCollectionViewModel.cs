using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Reactive.Disposables;
using Wada.AchievementEntry.Models;

namespace Wada.AchievementEntry.ViewModels;

public class AchievementCollectionViewModel : BindableBase, IDestructible
{
    private readonly AchievementCollectionModel _model;

    public AchievementCollectionViewModel(AchievementCollectionModel achievementCollectionModel)
    {

        _model = achievementCollectionModel;

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

    public void Destroy() => Disposables.Dispose();

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public ReactiveProperty<bool> CheckedItem { get; }

    public ReactiveProperty<DateTime> AchievementDate { get; }

    public ReactiveProperty<uint> EmployeeNumber { get; }
    
    public ReactiveProperty<string?> EmployeeName { get; }

    public ReadOnlyReactiveCollection<IValidationResultCollectionViewModel> ValidationResults { get; }
}
