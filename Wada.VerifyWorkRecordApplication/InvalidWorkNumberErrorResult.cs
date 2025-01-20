using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class InvalidWorkNumberErrorResult : IValidationErrorResult
{
    private InvalidWorkNumberErrorResult(string workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業台帳にない作業番号です";

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static InvalidWorkNumberErrorResult Create(string workOrderId, string jigCode, string note)
        => new(workOrderId, jigCode, note);

    public static InvalidWorkNumberErrorResult Parse(IValidationError validationResult)
    {
        if (validationResult is not InvalidWorkOrderIdError)
            throw new ArgumentException(
                $"引数には{nameof(InvalidWorkOrderIdError)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkOrderId.Value, validationResult.JigCode, validationResult.Note);
    }
}
