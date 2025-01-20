using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class WorkDateExpiredErrorResult : IValidationErrorResult
{
    private WorkDateExpiredErrorResult(string workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業日が完成を過ぎています";

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static WorkDateExpiredErrorResult Create(string workOrderId, string jigCode, string note)
        => new(workOrderId, jigCode, note);

    public static WorkDateExpiredErrorResult Parse(IValidationError validationResult)
    {
        if (validationResult is not WorkDateExpiredError)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredError)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkOrderId.Value, validationResult.JigCode, validationResult.Note);
    }
}
