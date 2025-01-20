using System.Transactions;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.DesignManagementWriter;
using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.AchieveTrackService.ProcessFlowAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkOrderAggregation;
using Wada.AOP.Logging;

namespace Wada.WriteWorkRecordApplication;

public interface IWriteWorkRecordUseCase
{
    Task<int> ExecuteAsync(IEnumerable<AchievementParam> achievements, bool canAddingDesignManagement);
}

public class WriteWorkRecordUseCase(IEmployeeRepository employeeReader,
                                    IProcessFlowRepository processFlowRepository,
                                    IWorkOrderRepository workOrderRepository,
                                    IAchievementLedgerRepository achievementLedgerRepository,
                                    IDesignManagementWriter designManagementWriter)
    : IWriteWorkRecordUseCase
{
    private const uint CadProcessFlowId = 2u;

    [Logging]
    public async Task<int> ExecuteAsync(IEnumerable<AchievementParam> achievements, bool canAddingDesignManagement)
    {
        // 最大実績ID取得
        var maxTask = achievementLedgerRepository.MaxByAchievementIdAsync();
        // 結合に必要な分だけ社員情報(部署ID)絞り込み
        var employeeTask = FetchEmployee(achievements);
        // 結合に必要な分だけ実績工程絞り込み
        var processFlowTask = FetchProcessFlow(achievements);
        // 結合に必要な分だけ作業台帳(自社番号)取得
        var workOrderTask = FetchWorkOrder(achievements);
        await Task.WhenAll(maxTask, employeeTask, processFlowTask, workOrderTask);

        // 実績IDインクリメントのため、最大値取得
        var maxAchievementLedger = maxTask.Result;

        // 登録に使用する情報を取得する
        var employees = employeeTask.Result;
        var processFlow = processFlowTask.Result;
        var workOrders = workOrderTask.Result;

        using TransactionScope scope = new();

        if (canAddingDesignManagement)
            WriteDesignManagement(achievements, processFlow, workOrders);

        var addedCount = WriteAchievementLedgers(achievements, maxAchievementLedger, employees, processFlow, workOrders);

        scope.Complete();
        return addedCount;
    }

    /// <summary>
    /// 実績台帳に追加する
    /// </summary>
    /// <param name="achievements"></param>
    /// <param name="maxAchievementLedger"></param>
    /// <param name="employees"></param>
    /// <param name="workOrders"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="WriteWorkRecordUseCaseException"></exception>
    private int WriteAchievementLedgers(IEnumerable<AchievementParam> achievements, AchievementLedger maxAchievementLedger, IEnumerable<Employee> employees, IEnumerable<ProcessFlow> processFlow, IEnumerable<WorkOrder> workOrders)
    {
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
                AchievementDetails = a.AchievementDetails.Join(
                    workOrders,
                    a => a.WorkOrderId,
                    w => w.WorkOrderId.Value,
                    (a, w) => new
                    {
                        w.OwnCompanyNumber,
                        a.WorkOrderId,
                        a.ManHour,
                        a.ProcessFlow,
                    })
                .Join(processFlow,
                a => a.ProcessFlow,
                p => p.Name,
                (a, p) => new
                {
                    a.OwnCompanyNumber,
                    a.WorkOrderId,
                    a.ManHour,
                    ProcessFlowId = p.Id,
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
                            x.ProcessFlowId,
                            x.ManHour)));

            });

        try
        {
            var addedCount = 0;
            // 実績台帳に追加する
            achievementLedgers.ToList().ForEach(x => addedCount += achievementLedgerRepository.Add(x));
            return addedCount;
        }
        catch (Exception ex) when (ex is AchievementLedgerAggregationException or NullReferenceException)
        {
            throw new WriteWorkRecordUseCaseException(
                $"実績を登録中に問題が発生しました\n{ex.Message}", ex);
        }
    }

    /// <summary>
    /// 設計管理に追加する
    /// </summary>
    /// <param name="achievements"></param>
    /// <param name="processFlow"></param>
    /// <param name="workOrders"></param>
    /// <exception cref="WriteWorkRecordUseCaseException"></exception>
    private void WriteDesignManagement(IEnumerable<AchievementParam> achievements, IEnumerable<ProcessFlow> processFlow, IEnumerable<WorkOrder> workOrders)
    {
        var workOrderIds =
            // 明細の実績工程がCADだけ抽出
            achievements.Select(x => new
            {
                x.WorkingDate,
                AchievementDetails = x.AchievementDetails.Join(processFlow.Where(x => x.Id == CadProcessFlowId),
                a => a.ProcessFlow,
                p => p.Name,
                (a, p) => new
                {
                    a.WorkOrderId,
                }),
            })
            // 親要素と子要素をジャグ配列にして一次配列にもどす
            .Select(x => x.AchievementDetails.Select(y => new
            {
                x.WorkingDate,
                y.WorkOrderId,
            }))
            .SelectMany(x => x)
            // 作業番号の重複を取り除く
            .GroupBy(x => x.WorkOrderId)
            .Select(x => new
            {
                WorkOrderId = x.Key,
                WorkingDate = x.Min(y => y.WorkingDate)
            });

        try
        {
            // 作業番号から自社番号を取得
            workOrderIds.Join(workOrders,
                                a => a.WorkOrderId,
                                w => w.WorkOrderId.Value,
                                (a, w) => new
                                {
                                    w.OwnCompanyNumber,
                                    a.WorkingDate,
                                })
                          .ToList().ForEach(
                            // 設計管理に登録する
                            x => _ = designManagementWriter.Add(x.OwnCompanyNumber, x.WorkingDate));
        }
        catch (DesignManagementWriterException ex)
        {
            throw new WriteWorkRecordUseCaseException(
                $"実績を登録中に問題が発生しました\n{ex.Message}", ex);
        }
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
            var employees = await Task.WhenAll(
                achievements.Select(
                    async x => await employeeReader.FindByEmployeeNumberAsync(x.EmployeeNumber)));
            return employees.Distinct();
        }
        catch (EmployeeNotFoundException ex)
        {
            throw new WriteWorkRecordUseCaseException(
                $"実績を登録中に問題が発生しました\n{ex.Message}");
        }
    }

    [Logging]
    private async Task<IEnumerable<ProcessFlow>> FetchProcessFlow(IEnumerable<AchievementParam> achievements)
    {
        try
        {
            var processFlows = achievements.Select(x => x.AchievementDetails.Select(y => y.ProcessFlow))
                                           .SelectMany(x => x)
                                           .Distinct();
            return await Task.WhenAll(processFlows.Select(x => processFlowRepository.FindByNameAsync(x)));
        }
        catch (ProcessFlowNotFoundException ex)
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
    private async Task<IEnumerable<WorkOrder>> FetchWorkOrder(IEnumerable<AchievementParam> achievements)
    {
        try
        {
            var WorkOrders = await Task.WhenAll(
                achievements.Select(
                    async (achievement, i) => await Task.WhenAll(
                        achievement.AchievementDetails.Select(
                            async x => await workOrderRepository.FindByWorkOrderIdAsync(
                                WorkOrderId.Create(x.WorkOrderId))))));
            return WorkOrders.SelectMany(x => x).Distinct();
        }
        catch (WorkOrderNotFoundException ex)
        {
            throw new WriteWorkRecordUseCaseException(
                $"実績を登録中に問題が発生しました\n{ex.Message}");
        }
    }
}

