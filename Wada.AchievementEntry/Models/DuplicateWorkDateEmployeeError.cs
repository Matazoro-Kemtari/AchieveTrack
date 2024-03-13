﻿using System;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

internal record class DuplicateWorkDateEmployeeError : IValidationError
{
    private DuplicateWorkDateEmployeeError(string workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "この作業日と社員番号の組み合わせが 実績処理で既に存在します";

    public string WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    internal static DuplicateWorkDateEmployeeError Create(string workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static DuplicateWorkDateEmployeeError Parse(IValidationErrorResult validationError)
    {
        if (validationError is not DuplicateWorkDateEmployeeErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(DuplicateWorkDateEmployeeErrorResult)}を渡してください",
                nameof(validationError));

        return Create(validationError.WorkingNumber, validationError.JigCode, validationError.Note);
    }
}