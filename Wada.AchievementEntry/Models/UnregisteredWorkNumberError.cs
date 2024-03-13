using System;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

internal record class UnregisteredWorkNumberError :  IValidationError
{
    private UnregisteredWorkNumberError(string workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }


    public string Message => "設計管理に未登録の作業番号があります";

    public string WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    internal static  UnregisteredWorkNumberError Create(string workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static UnregisteredWorkNumberError Parse(IValidationErrorResult validationError)
    {
        if (validationError is not UnregisteredWorkNumberErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkNumberErrorResult)}を渡してください",
                nameof(validationError));

        return Create(validationError.WorkingNumber, validationError.JigCode, validationError.Note);
    }
}
