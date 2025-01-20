using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

internal record class InvalidWorkOrderIdError : IValidationError
{
    private InvalidWorkOrderIdError(string workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業台帳に未登録の作業番号があります";

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }

    internal static InvalidWorkOrderIdError Create(string workOrderId, string jigCode, string note)
        => new(workOrderId, jigCode, note);

    public static InvalidWorkOrderIdError Parse(IValidationErrorResult validationResult)
    {
        if (validationResult is not InvalidWorkNumberErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(InvalidWorkNumberErrorResult)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkOrderId, validationResult.JigCode, validationResult.Note);
    }
}
