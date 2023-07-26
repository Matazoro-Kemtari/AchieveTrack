using System.Transactions;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.DesignManagementWriter;
using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;
using Wada.AOP.Logging;

namespace Wada.WriteWorkRecordApplication;

public interface IWriteWorkRecordUseCase
{
    Task<int> ExecuteAsync(IEnumerable<AchievementParam> achievements, bool canAddingDesignManagement);
}

public class WriteWorkRecordUseCase : IWriteWorkRecordUseCase
{
    private readonly IEmployeeReader _employeeReader;
    private readonly IWorkingLedgerReader _workingLedgerReader;
    private readonly IAchievementLedgerRepository _achievementLedgerRepository;
    private readonly IDesignManagementWriter _designManagementWriter;

    public WriteWorkRecordUseCase(IEmployeeReader employeeReader, IWorkingLedgerReader workingLedgerReader, IAchievementLedgerRepository achievementLedgerRepository, IDesignManagementWriter designManagementWriter)
    {
        _employeeReader = employeeReader;
        _workingLedgerReader = workingLedgerReader;
        _achievementLedgerRepository = achievementLedgerRepository;
        _designManagementWriter = designManagementWriter;
    }

    [Logging]
    public async Task<int> ExecuteAsync(IEnumerable<AchievementParam> achievements, bool canAddingDesignManagement)
    {
        var maxTask = _achievementLedgerRepository.MaxByAchievementIdAsync();
        var employeeTask = FetchEmployee(achievements);
        var workingLedgerTask = FetchWorkingLedger(achievements);
        await Task.WhenAll(maxTask, employeeTask, workingLedgerTask);

        // 実績IDインクリメントのため、最大値取得
        var maxAchievementLedger = maxTask.Result;

        // 登録に使用する情報を取得する
        //(Employee employee, WorkingLedger[] workingLedgers)[] additionalInformations = additionalInformationTask.Result;
        var employees = employeeTask.Result;
        var workingLedgers = workingLedgerTask.Result;

        using TransactionScope scope = new();

        if (canAddingDesignManagement)
        {
            try
            {
                // 作業番号の重複を取り除く
                var workingNumbers = achievements.Select(achievement => achievement.AchievementDetails.Select(x => x.WorkingNumber))
                                                 .SelectMany(x => x)
                                                 .Distinct();
                // 設計管理に登録する
                workingNumbers.Join(workingLedgers,
                                    a => a,
                                    w => w.WorkingNumber.Value,
                                    (a, w) => w.OwnCompanyNumber)
                              .ToList().ForEach(x => _ = _designManagementWriter.Add(x));
            }
            catch (DesignManagementWriterException ex)
            {
                throw new WriteWorkRecordUseCaseException(
                    $"実績を登録中に問題が発生しました\n{ex.Message}", ex);
            }
        }

        // Entity作成
        var achievementLedgers = achievements.Join(
            employees,
            a => a.EmployeeNumber,
            e => e.EmployeeNumber,
            (a, e) => new
            {
                a.WorkingDate,
                a.EmployeeNumber,
                e.DepartmentId,
                e.AchievementClassificationId,
                AchievementDetails = a.AchievementDetails.Join(
                    workingLedgers,
                    a => a.WorkingNumber,
                    w => w.WorkingNumber.Value,
                    (a, w) => new
                    {
                        w.OwnCompanyNumber,
                        a.WorkingNumber,
                        a.ManHour,
                    }),
            })
            .Select((achievement, index) =>
            {
                var nextAchievementId = maxAchievementLedger.AchievementId + 1u + (uint)index;
                return AchievementLedger.Create(
                    nextAchievementId,
                    achievement.WorkingDate,
                    achievement.EmployeeNumber,
                    achievement.DepartmentId,
                    achievement.AchievementDetails.Select(
                        x => AchievementDetail.Create(
                            nextAchievementId,
                            x.OwnCompanyNumber,
                            achievement.AchievementClassificationId ?? throw new NullReferenceException(),
                            x.ManHour)));

            });
        
        int addedCount = 0;
        try
        {
            // 実績台帳に追加する
            achievementLedgers.ToList().ForEach(x => addedCount += _achievementLedgerRepository.Add(x));
        }
        catch (Exception ex) when (ex is AchievementLedgerAggregationException or NullReferenceException)
        {
            throw new WriteWorkRecordUseCaseException(
                $"実績を登録中に問題が発生しました\n{ex.Message}", ex);
        }

        scope.Complete();
        return addedCount;
    }

    /// <summary>
    /// 社員情報を取得する
    /// </summary>
    /// <param name="achievements"></param>
    /// <returns></returns>
    /// <exception cref="WriteWorkRecordUseCaseException"></exception>
    [Logging]
    private async Task<IEnumerable<Employee>> FetchEmployee(IEnumerable<AchievementParam> achievements)
    {
        try
        {
            var employees = await Task.WhenAll(achievements.Select(async x => await _employeeReader.FindByEmployeeNumberAsync(x.EmployeeNumber)));
            return employees.Distinct();
        }
        catch (EmployeeAggregationException ex)
        {
            throw new WriteWorkRecordUseCaseException(
                $"実績を登録中に問題が発生しました\n{ex.Message}");
        }
    }

    /// <summary>
    /// 作業台帳を取得する
    /// </summary>
    /// <param name="achievements"></param>
    /// <returns></returns>
    /// <exception cref="WriteWorkRecordUseCaseException"></exception>
    [Logging]
    private async Task<IEnumerable<WorkingLedger>> FetchWorkingLedger(IEnumerable<AchievementParam> achievements)
    {
        try
        {
            var WorkingLedgers = await Task.WhenAll(achievements.Select(async (achievement, i) => await Task.WhenAll(achievement.AchievementDetails.Select(
                async x => await _workingLedgerReader.FindByWorkingNumberAsync(WorkingNumber.Create(x.WorkingNumber))))));
            return WorkingLedgers.SelectMany(x => x).Distinct();
        }
        catch (WorkingLedgerAggregationException ex)
        {
            throw new WriteWorkRecordUseCaseException(
                $"実績を登録中に問題が発生しました\n{ex.Message}");
        }
    }

    /// <summary>
    /// 社員情報と
    /// </summary>
    /// <param name="achievements"></param>
    /// <returns></returns>
    /// <exception cref="WriteWorkRecordUseCaseException"></exception>
    private Task<(Employee employee, WorkingLedger[] workingLedgers)[]> FetchEmployeeAndWorkingLedger(IEnumerable<AchievementParam> achievements)
    {
        return Task.WhenAll(achievements.Select(async (achievement, i) =>
        {
            // 部署IDと実績工程IDを取得する
            var employeeTask = _employeeReader.FindByEmployeeNumberAsync(achievement.EmployeeNumber);

            // 作業台帳を取得する
            var workingLedgerTasks = Task.WhenAll(
                achievement.AchievementDetails.Select(
                    async x => await _workingLedgerReader.FindByWorkingNumberAsync(WorkingNumber.Create(x.WorkingNumber))));

            try
            {
                await Task.WhenAll(employeeTask, workingLedgerTasks);
            }
            catch (DomainException ex) when (ex is EmployeeAggregationException
                                                   or WorkingLedgerAggregationException)
            {
                throw new WriteWorkRecordUseCaseException(
                    $"実績を登録中に問題が発生しました\n{ex.Message}");
            }

            var employee = employeeTask.Result;
            var workingLedgers = workingLedgerTasks.Result;
            return (employee, workingLedgers);
        }));
    }
}

