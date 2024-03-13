using System;
using Wada.AchieveTrackService.ValueObjects;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

public record class WorkDateExpiredError : IValidationError
{
    private WorkDateExpiredError(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業日が完成日を過ぎた作業番号があります";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static WorkDateExpiredError Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    internal static WorkDateExpiredError Create(string workingNumber, string jigCode, string note)
        => new(WorkingNumber.Create(workingNumber), jigCode, note);

    public static WorkDateExpiredError Parse(IValidationErrorResult validationError)
    {
        if (validationError is not WorkDateExpiredErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredErrorResult)}を渡してください",
                nameof(validationError));

        return Create(validationError.WorkingNumber, validationError.JigCode, validationError.Note);
    }
}
