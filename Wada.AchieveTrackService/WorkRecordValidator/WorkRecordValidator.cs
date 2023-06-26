﻿using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordReader;
using Wada.Data.OrderManagement.Models;
using Wada.Data.OrderManagement.Models.AchievementLedgerAggregation;
using Wada.Data.OrderManagement.Models.DesignManagementAggregation;
using Wada.Data.OrderManagement.Models.WorkingLedgerAggregation;

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

    public async Task<IEnumerable<IValidationResult>> ValidateWorkRecordsAsync(IEnumerable<WorkRecord> workRecords)
    {
        if (!workRecords.Any())
            throw new ArgumentNullException(nameof(workRecords));

        return await Task.WhenAll(workRecords.Select<WorkRecord, Task<IValidationResult>>(
            async x =>
            {
                if (!await IsWorkNumberInWorkingLedgerAsync(x.WorkingNumber))
                    return InvalidWorkNumberResult.Create();

                if (await IsWorkingDatePastCompletionAsync(x.WorkingNumber, x.WorkingDate))
                    return WorkDateExpiredResult.Create();

                if (await IsRecordInAchievementLedgerAsync(x.WorkingDate, x.EmployeeNumber))
                    return DuplicateWorkDateEmployeeResult.Create();

                if (!await IsWorkNumberInDesignManagementLedgerAsync(x.WorkingNumber))
                    return UnregisteredWorkNumberResult.Create();

                return ValidationSuccessResult.Create();
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
