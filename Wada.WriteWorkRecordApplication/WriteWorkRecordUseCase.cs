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
    private const uint CadProcessFlowId = 2u;
    private readonly IEmployeeRepository _employeeReader;
    private readonly IWorkingLedgerRepository _workingLedgerRepository;
    private readonly IAchievementLedgerRepository _achievementLedgerRepository;
    private readonly IDesignManagementWriter _designManagementWriter;

    public WriteWorkRecordUseCase(IEmployeeRepository employeeReader, IWorkingLedgerRepository workingLedgerRepository, IAchievementLedgerRepository achievementLedgerRepository, IDesignManagementWriter designManagementWriter)
    {
        _employeeReader = employeeReader;
        _workingLedgerRepository = workingLedgerRepository;
        _achievementLedgerRepository = achievementLedgerRepository;
        _designManagementWriter = designManagementWriter;
    }

    [Logging]
    public async Task<int> ExecuteAsync(IEnumerable<AchievementParam> achievements, bool canAddingDesignManagement)
    {
        var maxTask = _achievementLedgerRepository.MaxByAchievementIdAsync();
        var employeeTask = FetchEmployee(achievements);
        // TODO: 実績工程取得メソッド
        var workingLedgerTask = FetchWorkingLedger(achievements);
        await Task.WhenAll(maxTask, employeeTask, workingLedgerTask);

        // 実績IDインクリメントのため、最大値取得
        var maxAchievementLedger = maxTask.Result;

        // 登録に使用する情報を取得する
        var employees = employeeTask.Result;
        var workingLedgers = workingLedgerTask.Result;

        using TransactionScope scope = new();

        if (canAddingDesignManagement)
        {
            try
            {
                // 作業番号の重複を取り除く
                var workingNumbers = achievements.Join(employees,
                                                       a => a.EmployeeNumber,
                                                       e => e.EmployeeNumber,
                                                       (a, e) => new
                                                       {
                                                           e.ProcessFlowId,
                                                           a.WorkingDate,
                                                           a.AchievementDetails,
                                                       })
                                                 .Where(x => x.ProcessFlowId == CadProcessFlowId)
                                                 .Select(achievement => achievement.AchievementDetails.Select(x => new
                                                 {
                                                     achievement.WorkingDate,
                                                     x.WorkingNumber
                                                 }))
                                                 .SelectMany(x => x)
                                                 .GroupBy(x => x.WorkingNumber)
                                                 .Select(x => new
                                                 {
                                                     WorkingNumber = x.Key,
                                                     WorkingDate = x.Min(y => y.WorkingDate)
                                                 });


                // 設計管理に登録する
                workingNumbers.Join(workingLedgers,
                                    a => a.WorkingNumber,
                                    w => w.WorkingNumber.Value,
                                    (a, w) => new
                                    {
                                        w.OwnCompanyNumber,
                                        a.WorkingDate,
                                    })
                              .ToList().ForEach(x => _ = _designManagementWriter.Add(x.OwnCompanyNumber, x.WorkingDate));
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
                e.ProcessFlowId,
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
                var nextAchievementId = maxAchievementLedger.Id + 1u + (uint)index;
                return AchievementLedger.Create(
                    nextAchievementId,
                    achievement.WorkingDate,
                    achievement.EmployeeNumber,
                    achievement.DepartmentId,
                    achievement.AchievementDetails.Select(
                        x => AchievementDetail.Create(
                            nextAchievementId,
                            x.OwnCompanyNumber,
                            achievement.ProcessFlowId ?? throw new NullReferenceException(),
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
        catch (EmployeeNotFoundException ex)
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
            var WorkingLedgers = await Task.WhenAll(
                achievements.Select(
                    async (achievement, i) => await Task.WhenAll(
                        achievement.AchievementDetails.Select(
                            async x => await _workingLedgerRepository.FindByWorkingNumberAsync(
                                WorkingNumber.Create(x.WorkingNumber))))));
            return WorkingLedgers.SelectMany(x => x).Distinct();
        }
        catch (WorkingLedgerNotFoundException ex)
        {
            throw new WriteWorkRecordUseCaseException(
                $"実績を登録中に問題が発生しました\n{ex.Message}");
        }
    }
}

