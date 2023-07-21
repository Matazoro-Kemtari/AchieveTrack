using GongSolutions.Wpf.DragDrop;
using Livet.Messaging;
using Prism.Mvvm;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wada.AchievementEntry.Models;
using Wada.AchieveTrackService;
using Wada.IO;
using Wada.ReadWorkRecordApplication;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;
using Wada.WriteWorkRecordApplication;

namespace Wada.AchievementEntry.ViewModels;

public class AchievementEntryPageViewModel : BindableBase, IDestructible, IDropTarget
{
    private readonly AchievementEntryPageModel _model = new();
    private readonly IReadAchieveTrackUseCase _readAchieveTrackUseCase;
    private readonly IVerifyWorkRecordUseCase _verifyWorkRecordUseCase;
    private readonly IWriteWorkRecordUseCase _writeWorkRecordUseCase;

    public AchievementEntryPageViewModel(IReadAchieveTrackUseCase readAchieveTrackUseCase,
                                         IVerifyWorkRecordUseCase verifyWorkRecordUseCase,
                                         IWriteWorkRecordUseCase writeWorkRecordUseCase)
    {
        _readAchieveTrackUseCase = readAchieveTrackUseCase;
        _verifyWorkRecordUseCase = verifyWorkRecordUseCase;
        _writeWorkRecordUseCase = writeWorkRecordUseCase;

        // 日報エクセルリスト
        AchievementCollections = _model.AchievementCollections
            .ToReadOnlyReactiveCollection(x => x)
            .AddTo(Disposables);

        // 登録ボタン
        EntryCommand = new[]
        {
            AchievementCollections.ObserveProperty(x => x.Count).Select(x => x <= 0),
            // TODO: 上手くできない保留
            //AchievementCollections.Select(xx => xx.ValidationResults.ObserveProperty(x => x.Count).Select(x => x <= 0))
            //                      .ToObservable().SelectMany(x => x),
        }
        .CombineLatestValuesAreAllFalse()
        .ToAsyncReactiveCommand()
        .WithSubscribe(() => AddWorkRecordAsync())
        .AddTo(Disposables);

        // クリアボタン
        ClearCommand = new AsyncReactiveCommand()
            .WithSubscribe(() => Task.Run(() => _model.Clear()))
            .AddTo(Disposables);
    }

    public void Destroy() => Disposables.Dispose();

    public void DragOver(IDropInfo dropInfo)
    {
        var dragFiles = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
        dropInfo.Effects = dragFiles.Any(x => Path.GetExtension(x).ToLower() == ".xlsx")
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    public async void Drop(IDropInfo dropInfo)
    {
        var dragFiles = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
        dropInfo.Effects = dragFiles.Any(x => Path.GetExtension(x).ToLower() == ".xlsx")
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // 日報を読み込む
            IEnumerable<WorkRecordAttempt> workRecords;
            try
            {
                workRecords = await _readAchieveTrackUseCase.ExecuteAsync(dragFiles);

                // 日報を保持する
                _model.WorkRecords.AddRange(workRecords);
            }
            catch (FileStreamOpenerException ex)
            {
                var message = MessageNotificationViaLivet.MakeErrorMessage(ex.Message);
                await Messenger.RaiseAsync(message);
                Environment.Exit(0);
                return;
            }
            catch (DomainException ex)
            {
                var message = MessageNotificationViaLivet.MakeExclamationMessage(ex.Message);
                await Messenger.RaiseAsync(message);
                return;
            }

            // 検証
            IEnumerable<IEnumerable<IValidationResultAttempt>> validationResults;
            try
            {
                Mouse.OverrideCursor ??= Cursors.Wait;
                validationResults = await _verifyWorkRecordUseCase.ExecuteAsync(
                    workRecords.Select(
                        x => new WorkRecordParam(x.WorkingDate,
                                                 x.EmployeeNumber,
                                                 x.EmployeeName,
                                                 x.WorkingNumber,
                                                 x.ManHour)));
            }
            catch (Exception)
            {
                throw;
            }

            // 集計
            var parser = new Dictionary<Type, Func<IValidationResultAttempt, IValidationResultRequest>>
            {
                { typeof(DuplicateWorkDateEmployeeResultAttempt), DuplicateWorkDateEmployeeResultRequest.Parse },
                { typeof(InvalidWorkNumberResultAttempt), InvalidWorkNumberResultRequest.Parse },
                { typeof(UnregisteredWorkNumberResultAttempt), UnregisteredWorkNumberResultRequest.Parse },
                { typeof(WorkDateExpiredResultAttempt), WorkDateExpiredResultRequest.Parse },
            };

            // 日報と検証結果を結合する
            var merge = workRecords.Select((Value, i) => (Value, i)).Join(
                 validationResults.Select((Value, i) => (Value, i)),
                 r => r.i,
                 v => v.i,
                 (r, v) => new
                 {
                     r.Value.WorkingDate,
                     r.Value.WorkingNumber,
                     r.Value.EmployeeNumber,
                     r.Value.EmployeeName,
                     r.Value.ManHour,
                     ValidationResults = v.Value.Select(x => parser[x.GetType()](x)),
                 });

            // 集計する
            var vmCreater = new Dictionary<Type, Func<IValidationResultRequest, IValidationResultCollectionViewModel>>
            {
                { typeof(InvalidWorkNumberResultRequest), InvalidWorkNumberResultCollectionViewModel.Create },
                { typeof(DuplicateWorkDateEmployeeResultRequest), DuplicateWorkDateEmployeeResultCollectionViewModel.Create },
                { typeof(UnregisteredWorkNumberResultRequest), UnregisteredWorkNumberResultCollectionViewModel.Create },
                { typeof(WorkDateExpiredResultRequest), WorkDateExpiredResultCollectionViewModel.Create },
            };

            var aggregates = merge.GroupBy(x => new { x.WorkingDate, x.EmployeeNumber })
                .Select(g => new
                {
                    g.Key.WorkingDate,
                    g.Key.EmployeeNumber,
                    g.FirstOrDefault()?.EmployeeName,
                    ValidationResults = g.SelectMany(x => x.ValidationResults)
                                         .Distinct()
                                         .Select(x => vmCreater[x.GetType()](x)),
                });

            // ListViewのアイテム作成
            var collectionModels = aggregates.Select(
                x => new AchievementCollectionModel(x.WorkingDate,
                                                    x.EmployeeNumber,
                                                    x.EmployeeName,
                                                    x.ValidationResults));
            _model.AchievementCollections.AddRange(
                collectionModels.Select(x => new AchievementCollectionViewModel(x)));
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private async Task AddWorkRecordAsync()
    {
        // 引数作成
        var param = _model.WorkRecords.GroupBy(x => new { x.WorkingDate, x.EmployeeNumber })
            .Select(x => new AchievementParam(x.Key.WorkingDate,
                                              x.Key.EmployeeNumber,
                                              x.Select(y => new AchievementDetailParam(
                                                  y.WorkingNumber,
                                                  y.ManHour))));
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;
            await _writeWorkRecordUseCase.ExecuteAsync(param);
        }
        catch (WriteWorkRecordUseCaseException ex)
        {
            var message = MessageNotificationViaLivet.MakeErrorMessage(ex.Message);
            await Messenger.RaiseAsync(message);
            Environment.Exit(0);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    internal InteractionMessenger Messenger { get; } = new InteractionMessenger();

    public AsyncReactiveCommand EntryCommand { get; }

    public AsyncReactiveCommand ClearCommand { get; }

    /// <summary>
    /// 日報エクセルリスト
    /// </summary>
    public ReadOnlyReactiveCollection<AchievementCollectionViewModel> AchievementCollections { get; }
}
