using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class DuplicateWorkDateEmployeeErrorResult : IValidationErrorResult
{
    private DuplicateWorkDateEmployeeErrorResult(string workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "この作業日と社員番号の組み合わせが 実績処理で既に存在します";

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static DuplicateWorkDateEmployeeErrorResult Create(string workOrderId, string jigCode, string note)
        => new(workOrderId, jigCode, note);

    public static DuplicateWorkDateEmployeeErrorResult Parse(IValidationError validationResult)
    {
        if (validationResult is not DuplicateWorkDateEmployeeError)
            throw new ArgumentException(
                $"引数には{nameof(DuplicateWorkDateEmployeeError)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkOrderId.Value, validationResult.JigCode, validationResult.Note);
    }
}