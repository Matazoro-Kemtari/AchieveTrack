﻿using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class UnregisteredWorkNumberErrorResult : IValidationErrorResult
{
    private UnregisteredWorkNumberErrorResult(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "設計管理に未登録の作業番号です";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static UnregisteredWorkNumberErrorResult Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static UnregisteredWorkNumberErrorResult Parse(IValidationError validationResult)
    {
        if (validationResult is not UnregisteredWorkNumberError)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkNumberError)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }
}
