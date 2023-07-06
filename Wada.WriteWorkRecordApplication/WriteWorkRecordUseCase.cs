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
        var addedCounts = await Task.WhenAll(achievements.Select(async achievement =>
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

            // Entity作成
            var achievementLedger = AchievementLedger.Create(achievement.WorkingDate,
                                                             achievement.EmployeeNumber,
                                                             achievement.AchievementDetails.Select(
                                                                 x => AchievementDetail.Create(
                                                                 workingLedgers.First(y => y.WorkingNumber.Value == x.WorkingNumber).OwnCompanyNumber,
                                                                 x.ManHour)));
            try
            {
                // 実績台帳に追加する
                return await _achievementLedgerRepository.AddAsync(achievementLedger);
            }
            catch (AchievementLedgerAggregationException ex)
            {
                throw new WriteWorkRecordUseCaseException(
                    $"実績を登録中に問題が発生しました\n{ex.Message}", ex);
            }
        }));
        return addedCounts.Sum();
    }
}

