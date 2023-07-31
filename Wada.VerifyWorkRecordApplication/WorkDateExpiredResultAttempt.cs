using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class WorkDateExpiredResultAttempt : WorkDateExpiredResult, IValidationResultAttempt
{
    protected WorkDateExpiredResultAttempt(WorkingNumber workingNumber, string jigCode, string note)
        : base(workingNumber, jigCode, note)
    { }

    private static new WorkDateExpiredResultAttempt Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static WorkDateExpiredResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not WorkDateExpiredResult)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredResult)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }
}
