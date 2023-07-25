using System.Transactions;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;
using Wada.AOP.Logging;

namespace Wada.WriteWorkRecordApplication;

public interface IWriteWorkRecordUseCase
{
    Task<int> ExecuteAsync(IEnumerable<AchievementParam> achievements);
}

public class WriteWorkRecordUseCase : IWriteWorkRecordUseCase
{
    private readonly IEmployeeReader _employeeReader;
    private readonly IWorkingLedgerReader _workingLedgerReader;
    private readonly IAchievementLedgerRepository _achievementLedgerRepository;

    public WriteWorkRecordUseCase(IEmployeeReader employeeReader, IWorkingLedgerReader workingLedgerReader, IAchievementLedgerRepository achievementLedgerRepository)
    {
        _employeeReader = employeeReader;
        _workingLedgerReader = workingLedgerReader;
        _achievementLedgerRepository = achievementLedgerRepository;
    }

    [Logging]
    public async Task<int> ExecuteAsync(IEnumerable<AchievementParam> achievements)
    {
        var maxTask = _achievementLedgerRepository.MaxByAchievementIdAsync();
        var additionalInformationTask = FetchEmployeeAndWorkingLedger(achievements);
        await Task.WhenAll(maxTask, additionalInformationTask);

        // 実績IDインクリメントのため、全件取得
        var maxAchievementLedger = maxTask.Result;

        // 登録に使用する情報を取得する
        (Employee employee, WorkingLedger[] workingLedgers)[] additionalInformations = additionalInformationTask.Result;

        using TransactionScope scope = new();

        int addedCount = 0;
        uint index = 0;
        achievements.ToList().ForEach(achievement =>
        {
            try
            {
                var nextAchievementId = maxAchievementLedger.AchievementId + 1u + index;
                // Entity作成
                var achievementLedger = AchievementLedger.Create(
                nextAchievementId,
                    achievement.WorkingDate,
                    achievement.EmployeeNumber,
                    additionalInformations[index].employee.DepartmentId,
                    achievement.AchievementDetails.Select(
                        x => AchievementDetail.Create(
                            nextAchievementId,
                            additionalInformations[index].workingLedgers.First(y => y.WorkingNumber.Value == x.WorkingNumber).OwnCompanyNumber,
                            additionalInformations[index].employee.AchievementClassificationId ?? throw new NullReferenceException(),
                            x.ManHour)));

                // 実績台帳に追加する
                addedCount += _achievementLedgerRepository.Add(achievementLedger);
            }
            catch (Exception ex) when (ex is AchievementLedgerAggregationException or NullReferenceException)
            {
                throw new WriteWorkRecordUseCaseException(
                    $"実績を登録中に問題が発生しました\n{ex.Message}", ex);
            }
            index++;
        });

        scope.Complete();
        return addedCount;
    }

    /// <summary>
    /// 社員情報と作業台帳を取得する
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

