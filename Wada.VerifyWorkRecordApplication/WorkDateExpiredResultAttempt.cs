using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class WorkDateExpiredResultAttempt : WorkDateExpiredResult, IValidationResultAttempt
{
    protected WorkDateExpiredResultAttempt(WorkingNumber workingNumber, string note)
        : base(workingNumber, note)
    { }

    private static new WorkDateExpiredResultAttempt Create(WorkingNumber workingNumber, string note)
        => new(workingNumber, note);

    public static WorkDateExpiredResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not WorkDateExpiredResult)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredResult)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.Note);
    }
}
