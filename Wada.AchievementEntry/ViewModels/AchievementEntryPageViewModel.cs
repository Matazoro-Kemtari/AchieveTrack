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
using Wada.AchieveTrackService.WorkRecordValidator;
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

        // 設計管理登録チェックボックス
        AddingDesignManagementIsChecked = _model.AddingDesignManagementIsChecked
            .AddTo(Disposables);

        // 登録ボタン
        EntryCommand = new[]
        {
            AchievementCollections.ObserveProperty(x => x.Count).Select(x => x <= 0),
            // エラーがあったら無効 　　出来ないから保留
            //AddingDesignManagementIsChecked.Value
            //? AchievementCollections.ObserveProperty(
            //    x => x.Select(y => y.ValidationResults.Where(z => z.GetType() != typeof(UnregisteredWorkNumberResultCollectionViewModel))
            //                            .Any())
            //    .Any())
            
            //: AchievementCollections.Select(x => x.ValidationResults.Any()).ToObservable(),
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
            _model.Clear();

            // 日報を読み込む
            IEnumerable<WorkRecordAttempt>? workRecords = await ReadAchieveTrack(dragFiles);
            if (workRecords == null)
                return;

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
                                                 x.Note,
                                                 x.ManHour)));
            }
            catch (WorkRecordValidatorException ex)
            {
                var message = MessageNotificationViaLivet.MakeErrorMessage(ex.Message);
                await Messenger.RaiseAsync(message);
                Environment.Exit(0);
                return;
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

    private async Task<IEnumerable<WorkRecordAttempt>?> ReadAchieveTrack(IEnumerable<string> paths)
    {
        try
        {
            var workRecords = await _readAchieveTrackUseCase.ExecuteAsync(paths);

            // 日報を保持する
            _model.WorkRecords.AddRange(workRecords);

            return workRecords;
        }
        catch (FileStreamOpenerException ex)
        {
            var message = MessageNotificationViaLivet.MakeErrorMessage(ex.Message);
            await Messenger.RaiseAsync(message);
            Environment.Exit(0);
            return null;
        }
        catch (Exception ex) when (ex is DomainException or ReadAchieveTrackUseCaseException)
        {
            var message = MessageNotificationViaLivet.MakeExclamationMessage(ex.Message);
            await Messenger.RaiseAsync(message);
            return null;
        }
    }

    private async Task AddWorkRecordAsync()
    {
        // ボタンの有効判定で実現したい
        if (!_model.AchievementCollections.Any(x => x.CheckedItem.Value))
        {
            var message = MessageNotificationViaLivet.MakeExclamationMessage(
                "1つも選択されていないため実行できません");
            await Messenger.RaiseAsync(message);
            return;
        }

        if (_model.AddingDesignManagementIsChecked.Value)
        {
            if (_model.AchievementCollections.Where(x => x.CheckedItem.Value)
                                             .Select(
                x => x.ValidationResults.Where(
                    y => y.GetType() != typeof(UnregisteredWorkNumberResultCollectionViewModel))
                                        .Any())
                                             .Any(x => x))
            {
                var message = MessageNotificationViaLivet.MakeExclamationMessage(
                    "エラーがあるため実行できません");
                await Messenger.RaiseAsync(message);
                return;
            }
        }
        else
        {
            if (_model.AchievementCollections.Where(x => x.CheckedItem.Value)
                                             .Select(
                x => x.ValidationResults.Any())
                                             .Any(x => x))
            {
                var message = MessageNotificationViaLivet.MakeExclamationMessage(
                    "エラーがあるため実行できません");
                await Messenger.RaiseAsync(message);
                return;
            }
        }

        // 引数作成
        var param = _model.WorkRecords.Join(
            _model.AchievementCollections.Where(x => x.CheckedItem.Value),
            w => new { w.WorkingDate, w.EmployeeNumber },
            a => new { WorkingDate = a.AchievementDate.Value, EmployeeNumber = a.EmployeeNumber.Value },
            (w, _) => new
            {
                w.WorkingDate,
                w.EmployeeNumber,
                w.WorkingNumber,
                w.ManHour,
            })
            .GroupBy(x => new { x.WorkingDate, x.EmployeeNumber })
            .Select(x => new AchievementParam(x.Key.WorkingDate,
                                              x.Key.EmployeeNumber,
                                              x.GroupBy(y => y.WorkingNumber).Select(y => new AchievementDetailParam(
                                                  y.Key,
                                                  y.Sum(z => z.ManHour)))));

        try
        {
            Mouse.OverrideCursor = Cursors.Wait;
            await _writeWorkRecordUseCase.ExecuteAsync(param, AddingDesignManagementIsChecked.Value);

            var message = MessageNotificationViaLivet.MakeInformationMessage("登録しました");
            await Messenger.RaiseAsync(message);
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
        _model.Clear();
    }

    /// <summary>
    /// Disposeが必要なReactivePropertyやReactiveCommandを集約させるための仕掛け
    /// </summary>
    private CompositeDisposable Disposables { get; } = new CompositeDisposable();

    public InteractionMessenger Messenger { get; } = new InteractionMessenger();

    public AsyncReactiveCommand EntryCommand { get; }

    public AsyncReactiveCommand ClearCommand { get; }

    /// <summary>
    /// 日報エクセルリスト
    /// </summary>
    public ReadOnlyReactiveCollection<AchievementCollectionViewModel> AchievementCollections { get; }

    /// <summary>
    /// 設計情報に追加するチェックボックス
    /// </summary>
    public ReactivePropertySlim<bool> AddingDesignManagementIsChecked { get; }
}
