using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class WorkDateExpiredErrorResult : IValidationErrorResult
{
    private WorkDateExpiredErrorResult(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業日が完成を過ぎています";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static WorkDateExpiredErrorResult Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static WorkDateExpiredErrorResult Parse(IValidationError validationResult)
    {
        if (validationResult is not WorkDateExpiredError)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredError)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }
}
