using GongSolutions.Wpf.DragDrop;
using Livet.Messaging;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Wada.AchievementEntry.Models;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.AOP.Logging;
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
            .ToReactiveProperty()
            .AddTo(Disposables);
        _ = AddingDesignManagementIsChecked.Subscribe(_ => CanRegister()).AddTo(Disposables);

        // 選択チェックを購読
        _ = _model.AchievementCollections
            .ObserveElementObservableProperty(x => x.CheckedItem)
            .Subscribe(x => CanRegister())
            .AddTo(Disposables);

        // 登録ボタン
        EntryCommand = AchievementsAnySelected
        .ToAsyncReactiveCommand()
        .WithSubscribe(() => AddWorkRecordAsync())
        .AddTo(Disposables);

        // クリアボタン
        ClearCommand = new ReactiveCommand()
            .WithSubscribe(() => _model.Clear())
            .AddTo(Disposables);
    }

    [Logging]
    private void CanRegister()
    {
        if (_model.AchievementCollections.Any(x => x.CheckedItem.Value))
        {
            // 1つ以上選択のチェックが入っている

            if (AddingDesignManagementIsChecked.Value)
            {
                // 設計管理に登録する

                // 選択チェックはONで、設計管理に未登録を除いたエラー内容がある
                var hasError = _model.AchievementCollections.Where(x => x.CheckedItem.Value)
                                                            .Any(x => x.HasErrorsWithOutDesignManagement.Value);
                AchievementsAnySelected.Value = !hasError;
            }
            else
            {
                // いずれかのエラー内容がある
                var hasError = _model.AchievementCollections.Where(x => x.CheckedItem.Value)
                                                            .Any(x => x.HasErrors.Value);
                AchievementsAnySelected.Value = !hasError;
            }
        }
        else
            AchievementsAnySelected.Value = false;
    }

    public void Destroy() => Disposables.Dispose();

    [Logging]
    public void DragOver(IDropInfo dropInfo)
    {
        var dragFiles = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
        dropInfo.Effects = dragFiles.Any(x => Path.GetExtension(x).ToLower() == ".xlsx")
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    [Logging]
    public async void Drop(IDropInfo dropInfo)
    {
        var dragFiles = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
        dropInfo.Effects = dragFiles.Any(x => Path.GetExtension(x).ToLower() == ".xlsx")
            ? DragDropEffects.Copy
            : DragDropEffects.None;

        try
        {
            var actionMessage = MessageNotificationViaLivet.MakeWindowActiveMessage();
            await Messenger.RaiseAsync(actionMessage);

            Mouse.OverrideCursor = Cursors.Wait;
            _model.Clear();

            // 日報を読み込む
            IEnumerable<WorkRecordResult>? workRecords = await ReadAchieveTrack(dragFiles);
            if (workRecords == null)
                return;

            // 検証
            IEnumerable<IEnumerable<IValidationErrorResult>> validationResults;
            try
            {
                Mouse.OverrideCursor ??= Cursors.Wait;
                validationResults = await _verifyWorkRecordUseCase.ExecuteAsync(
                    workRecords.Select(
                        x => new WorkRecordParam(x.WorkingDate,
                                                 x.EmployeeNumber,
                                                 x.EmployeeName,
                                                 x.WorkOrderId,
                                                 x.JigCode,
                                                 x.ProcessFlow,
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
            var parser = new Dictionary<Type, Func<IValidationErrorResult, Models.IValidationError>>
            {
                { typeof(DuplicateWorkDateEmployeeErrorResult), Models.DuplicateWorkDateEmployeeError.Parse },
                { typeof(InvalidWorkNumberErrorResult), Models.InvalidWorkOrderIdError.Parse },
                { typeof(UnregisteredWorkOrderIdErrorResult), Models.UnregisteredWorkOrderIdError.Parse },
                { typeof(WorkDateExpiredErrorResult), Models.WorkDateExpiredError.Parse },
            };

            // 日報と検証結果を結合する
            var merge = workRecords.Select((Value, i) => (Value, i)).Join(
                 validationResults.Select((Value, i) => (Value, i)),
                 r => r.i,
                 v => v.i,
                 (r, v) => new
                 {
                     r.Value.WorkingDate,
                     r.Value.WorkOrderId,
                     r.Value.EmployeeNumber,
                     r.Value.EmployeeName,
                     r.Value.ManHour,
                     ValidationResults = v.Value.Select(x => parser[x.GetType()](x)),
                 });

            // 集計する
            var vmCreater = new Dictionary<Type, Func<Models.IValidationError, IValidationErrorCollectionViewModel>>
            {
                { typeof(Models.InvalidWorkOrderIdError), InvalidWorkNumberErrorCollectionViewModel.Create },
                { typeof(Models.DuplicateWorkDateEmployeeError), DuplicateWorkDateEmployeeErrorCollectionViewModel.Create },
                { typeof(Models.UnregisteredWorkOrderIdError), UnregisteredWorkOrderIdErrorCollectionViewModel.Create },
                { typeof(Models.WorkDateExpiredError), WorkDateExpiredErrorCollectionViewModel.Create },
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
                collectionModels.Select(x => AchievementCollectionViewModel.Create(x)));
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    [Logging]
    private async Task<IEnumerable<WorkRecordResult>?> ReadAchieveTrack(IEnumerable<string> paths)
    {
        try
        {
            var workRecords = await _readAchieveTrackUseCase.ExecuteAsync(paths);

            // 日報を保持する
            _model.WorkRecords.AddRange(workRecords);

            return workRecords;
        }
        catch (AchieveTrackIOException ex)
        {
            var message = MessageNotificationViaLivet.MakeErrorMessage(ex.Message);
            await Messenger.RaiseAsync(message);
            Environment.Exit(0);
            return null;
        }
        catch (ReadAchieveTrackUseCaseException ex)
        {
            var message = MessageNotificationViaLivet.MakeExclamationMessage(ex.Message);
            await Messenger.RaiseAsync(message);
            return null;
        }
    }

    [Logging]
    private async Task AddWorkRecordAsync()
    {
        // 引数作成
        var param = _model.WorkRecords.Join(
            _model.AchievementCollections.Where(x => x.CheckedItem.Value),
            w => new { w.WorkingDate, w.EmployeeNumber },
            a => new { WorkingDate = a.AchievementDate.Value, EmployeeNumber = a.EmployeeNumber.Value },
            (w, _) => new
            {
                w.WorkingDate,
                w.EmployeeNumber,
                w.WorkOrderId,
                w.ProcessFlow,
                w.ManHour,
            })
            .GroupBy(x => new { x.WorkingDate, x.EmployeeNumber })
            .Select(x => new AchievementParam(x.Key.WorkingDate,
                                              x.Key.EmployeeNumber,
                                              x.GroupBy(y => y.WorkOrderId).Select(y => new AchievementDetailParam(
                                                  y.Key,
                                                  y.Select(z => z.ProcessFlow).First(), // ここでエラーが起こるなら取込時に処理に問題あり
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

    public ReactiveCommand ClearCommand { get; }

    /// <summary>
    /// 日報エクセルリスト
    /// </summary>
    public ReadOnlyReactiveCollection<AchievementCollectionViewModel> AchievementCollections { get; }

    /// <summary>
    /// 設計情報に追加するチェックボックス
    /// </summary>
    public ReactiveProperty<bool> AddingDesignManagementIsChecked { get; }

    /// <summary>
    /// 転送可能な実績内容が選択されているか
    /// </summary>
    private ReactiveProperty<bool> AchievementsAnySelected { get; } = new(false);
}
