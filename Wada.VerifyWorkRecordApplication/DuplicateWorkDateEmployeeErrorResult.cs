using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class DuplicateWorkDateEmployeeErrorResult : IValidationErrorResult
{
    private DuplicateWorkDateEmployeeErrorResult(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "この作業日と社員番号の組み合わせが 実績処理で既に存在します";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static DuplicateWorkDateEmployeeErrorResult Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static DuplicateWorkDateEmployeeErrorResult Parse(IValidationError validationResult)
    {
        if (validationResult is not DuplicateWorkDateEmployeeError)
            throw new ArgumentException(
                $"引数には{nameof(DuplicateWorkDateEmployeeError)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }
}