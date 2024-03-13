using System;
using Wada.AchieveTrackService.ValueObjects;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

public record class UnregisteredWorkNumberError :  IValidationError
{
    private UnregisteredWorkNumberError(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }


    public string Message => "設計管理に未登録の作業番号があります";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static new UnregisteredWorkNumberError Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    internal static UnregisteredWorkNumberError Create(string workingNumber, string jigCode, string note)
        => new(WorkingNumber.Create(workingNumber), jigCode, note);

    public static UnregisteredWorkNumberError Parse(IValidationErrorResult validationError)
    {
        if (validationError is not UnregisteredWorkNumberErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkNumberErrorResult)}を渡してください",
                nameof(validationError));

        return Create(validationError.WorkingNumber, validationError.JigCode, validationError.Note);
    }
}
