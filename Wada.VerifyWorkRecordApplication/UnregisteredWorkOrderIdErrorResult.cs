using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class UnregisteredWorkOrderIdErrorResult : IValidationErrorResult
{
    private UnregisteredWorkOrderIdErrorResult(string workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "設計管理に未登録の作業番号です";

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static UnregisteredWorkOrderIdErrorResult Create(string workOrderId, string jigCode, string note)
        => new(workOrderId, jigCode, note);

    public static UnregisteredWorkOrderIdErrorResult Parse(IValidationError validationResult)
    {
        if (validationResult is not UnregisteredWorkOrderIdError)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkOrderIdError)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkOrderId.Value, validationResult.JigCode, validationResult.Note);
    }
}
