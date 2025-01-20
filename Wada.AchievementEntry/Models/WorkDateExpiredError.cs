using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

internal record class WorkDateExpiredError : IValidationError
{
    private WorkDateExpiredError(string workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業日が完成日を過ぎた作業番号があります";

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }

    internal static WorkDateExpiredError Create(string workOrderId, string jigCode, string note)
        => new(workOrderId, jigCode, note);

    public static WorkDateExpiredError Parse(IValidationErrorResult validationError)
    {
        if (validationError is not WorkDateExpiredErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredErrorResult)}を渡してください",
                nameof(validationError));

        return Create(validationError.WorkOrderId, validationError.JigCode, validationError.Note);
    }
}
