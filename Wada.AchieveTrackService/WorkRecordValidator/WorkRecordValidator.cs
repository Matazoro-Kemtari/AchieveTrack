using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.DesignManagementAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;
using Wada.AchieveTrackService.WorkRecordReader;

namespace Wada.AchieveTrackService.WorkRecordValidator;

public class WorkRecordValidator : IWorkRecordValidator
{
    private readonly IWorkingLedgerRepository _workingLedgerRepository;
    private readonly IAchievementLedgerRepository _achievementLedgerRepository;
    private readonly IDesignManagementRepository _designManagementRepository;

    public WorkRecordValidator(IWorkingLedgerRepository workingLedgerRepository, IAchievementLedgerRepository achievementLedgerRepository, IDesignManagementRepository designManagementRepository)
    {
        _workingLedgerRepository = workingLedgerRepository;
        _achievementLedgerRepository = achievementLedgerRepository;
        _designManagementRepository = designManagementRepository;
    }

    public async Task<IEnumerable<IEnumerable<IValidationResult>>> ValidateWorkRecordsAsync(IEnumerable<WorkRecord> workRecords)
    {
        if (!workRecords.Any())
            throw new ArgumentNullException(nameof(workRecords));

        return await Task.WhenAll(workRecords.Select(
            async x =>
            {
                var validationResults = new List<IValidationResult>();

                if (await IsWorkNumberInWorkingLedgerAsync(x.WorkingNumber))
                {
                    if (await IsWorkingDatePastCompletionAsync(x.WorkingNumber, x.WorkingDate))
                        validationResults.Add(WorkDateExpiredResult.Create(x.WorkingNumber, x.Note));

                    if (!await IsWorkNumberInDesignManagementLedgerAsync(x.WorkingNumber))
                        validationResults.Add(UnregisteredWorkNumberResult.Create(x.WorkingNumber, x.Note));
                }
                else
                    validationResults.Add(InvalidWorkNumberResult.Create(x.WorkingNumber, x.Note));

                if (await IsRecordInAchievementLedgerAsync(x.WorkingDate, x.EmployeeNumber))
                    validationResults.Add(DuplicateWorkDateEmployeeResult.Create(x.WorkingNumber, x.Note));

                return validationResults;
            }));
    }

    /// <summary>
    /// 作業番号が作業台帳にあるか調べる
    /// </summary>
    /// <param name="workingNumber"></param>
    /// <returns>存在する場合: true</returns>
    private async Task<bool> IsWorkNumberInWorkingLedgerAsync(WorkingNumber workingNumber)
    {
        try
        {
            _ = await _workingLedgerRepository.FindByWorkingNumberAsync(workingNumber);
            return true;
        }
        catch (WorkingLedgerAggregationException)
        {
            return false;
        }
    }

    /// <summary>
    /// 作業日が完成日を過ぎているか調べる
    /// </summary>
    /// <param name="workingDate">過ぎている場合: true</param>
    /// <returns></returns>
    private async Task<bool> IsWorkingDatePastCompletionAsync(WorkingNumber workingNumber, DateTime workingDate)
    {
        try
        {
            var result = await _workingLedgerRepository.FindByWorkingNumberAsync(workingNumber);
            return result.CompletionDate != null
                   && workingDate.CompareTo(result.CompletionDate) > 0;
        }
        catch (WorkingLedgerAggregationException ex)
        {
            throw new WorkRecordValidatorException(ex.Message, ex);
        }
    }

    /// <summary>
    /// 実績台帳に登録済みか調べる
    /// </summary>
    /// <param name="workNumber"></param>
    /// <returns>存在する場合: true</returns>
    private async Task<bool> IsRecordInAchievementLedgerAsync(DateTime workingDate, uint employeeNumber)
    {
        try
        {
            _ = await _achievementLedgerRepository.FindByWorkingDateAndEmployeeNumberAsync(workingDate, employeeNumber);
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
    private async Task<bool> IsWorkNumberInDesignManagementLedgerAsync(WorkingNumber workNumber)
    {
        try
        {
            var workingLedger = await _workingLedgerRepository.FindByWorkingNumberAsync(workNumber);
            _ = await _designManagementRepository.FindByOwnCompanyNumberAsync(workingLedger.OwnCompanyNumber);
            return true;
        }
        catch (DesignManagementAggregationException)
        {
            return false;
        }
    }
}
