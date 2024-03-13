using System;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

internal record class WorkDateExpiredError : IValidationError
{
    private WorkDateExpiredError(string workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業日が完成日を過ぎた作業番号があります";

    public string WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    internal static WorkDateExpiredError Create(string workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static WorkDateExpiredError Parse(IValidationErrorResult validationError)
    {
        if (validationError is not WorkDateExpiredErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredErrorResult)}を渡してください",
                nameof(validationError));

        return Create(validationError.WorkingNumber, validationError.JigCode, validationError.Note);
    }
}
