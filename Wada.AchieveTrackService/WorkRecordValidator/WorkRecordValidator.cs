using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.DesignManagementAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkOrderAggregation;
using Wada.AchieveTrackService.WorkRecordReader;
using Wada.AOP.Logging;

namespace Wada.AchieveTrackService.WorkRecordValidator;

public class WorkRecordValidator(IWorkOrderRepository workOrderRepository,
                                 IAchievementLedgerRepository achievementLedgerRepository,
                                 IDesignManagementRepository designManagementRepository)
    : IWorkRecordValidator
{
    [Logging]
    public async Task<IEnumerable<IEnumerable<IValidationError>>> ValidateWorkRecordsAsync(IEnumerable<WorkRecord> workRecords)
    {
        if (!workRecords.Any())
            throw new ArgumentNullException(nameof(workRecords));

        const string CadProcessFlow = "CAD";

        return await Task.WhenAll((IEnumerable<Task<List<IValidationError>>>)workRecords.Select(
            async x =>
            {
                var validationResults = new List<IValidationError>();

                if (await IsWorkNumberInWorkOrderAsync(x.WorkOrderId))
                {
                    if (await IsWorkingDatePastCompletionAsync(x.WorkOrderId, x.WorkingDate))
                        validationResults.Add(WorkDateExpiredError.Create(x.WorkOrderId, x.JigCode, x.Note));

                    if (x.ProcessFlow == CadProcessFlow
                        && !await IsWorkNumberInDesignManagementLedgerAsync(x.WorkOrderId))
                        validationResults.Add(UnregisteredWorkOrderIdError.Create(x.WorkOrderId, x.JigCode, x.Note));
                }
                else
                    validationResults.Add(InvalidWorkOrderIdError.Create(x.WorkOrderId, x.JigCode, x.Note));

                if (await IsRecordInAchievementLedgerAsync(x.WorkingDate, x.EmployeeNumber))
                    validationResults.Add(DuplicateWorkDateEmployeeError.Create(x.WorkOrderId, x.JigCode, x.Note));

                return validationResults;
            }));
    }

    /// <summary>
    /// 作業番号が作業台帳にあるか調べる
    /// </summary>
    /// <param name="workOrderId"></param>
    /// <returns>存在する場合: true</returns>
    [Logging]
    private async Task<bool> IsWorkNumberInWorkOrderAsync(WorkOrderId workOrderId)
    {
        try
        {
            _ = await workOrderRepository.FindByWorkOrderIdAsync(workOrderId);
            return true;
        }
        catch (WorkOrderNotFoundException)
        {
            return false;
        }
    }

    /// <summary>
    /// 作業日が完成日を過ぎているか調べる
    /// </summary>
    /// <param name="workingDate">過ぎている場合: true</param>
    /// <returns></returns>
    [Logging]
    private async Task<bool> IsWorkingDatePastCompletionAsync(WorkOrderId workOrderId, DateTime workingDate)
    {
        try
        {
            var result = await workOrderRepository.FindByWorkOrderIdAsync(workOrderId);
            return result.CompletionDate != null
                   && workingDate.CompareTo(result.CompletionDate) > 0;
        }
        catch (WorkOrderNotFoundException ex)
        {
            throw new WorkRecordValidatorException(ex.Message, ex);
        }
    }

    /// <summary>
    /// 実績台帳に登録済みか調べる
    /// </summary>
    /// <param name="workNumber"></param>
    /// <returns>存在する場合: true</returns>
    [Logging]
    private async Task<bool> IsRecordInAchievementLedgerAsync(DateTime workingDate, uint employeeNumber)
    {
        try
        {
            _ = await achievementLedgerRepository.FindByWorkingDateAndEmployeeNumberAsync(workingDate, employeeNumber);
            return true;
        }
        catch (AchievementLedgerAggregationException)
        {
            return false;
        }
    }

    /// <summary>
    /// 設計管理に登録済みか調べる
    /// </summary>
    /// <param name="workNumber"></param>
    /// <returns>存在する場合: true</returns>
    [Logging]
    private async Task<bool> IsWorkNumberInDesignManagementLedgerAsync(WorkOrderId workNumber)
    {
        try
        {
            var workOrder = await workOrderRepository.FindByWorkOrderIdAsync(workNumber);
            _ = await designManagementRepository.FindByOwnCompanyNumberAsync(workOrder.OwnCompanyNumber);
            return true;
        }
        catch (Exception ex) when (ex is WorkOrderNotFoundException or DesignManagementNotFoundException)
        {
            return false;
        }
    }
}
