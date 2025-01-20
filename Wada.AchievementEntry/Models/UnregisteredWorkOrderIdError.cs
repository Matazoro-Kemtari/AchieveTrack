using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

internal record class UnregisteredWorkOrderIdError : IValidationError
{
    private UnregisteredWorkOrderIdError(string workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }


    public string Message => "設計管理に未登録の作業番号があります";

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }

    internal static UnregisteredWorkOrderIdError Create(string workOrderId, string jigCode, string note)
        => new(workOrderId, jigCode, note);

    public static UnregisteredWorkOrderIdError Parse(IValidationErrorResult validationError)
    {
        if (validationError is not UnregisteredWorkOrderIdErrorResult)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkOrderIdErrorResult)}を渡してください",
                nameof(validationError));

        return Create(validationError.WorkOrderId, validationError.JigCode, validationError.Note);
    }
}
