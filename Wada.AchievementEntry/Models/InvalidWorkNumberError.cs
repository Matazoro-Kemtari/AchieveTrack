using System;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

internal record class InvalidWorkNumberError : IValidationError
{
    private InvalidWorkNumberError(string workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業台帳に未登録の作業番号があります";

    public string WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    internal static InvalidWorkNumberError Create(string workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static InvalidWorkNumberError Parse(IValidationErrorResult validationResult)
    {
        if (validationResult is not InvalidWorkNumberErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(InvalidWorkNumberErrorResult)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }
}
