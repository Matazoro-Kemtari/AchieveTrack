using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class WorkDateExpiredResultAttempt : WorkDateExpiredResult, IValidationResultAttempt
{
    private WorkDateExpiredResultAttempt() { }

    private static new WorkDateExpiredResultAttempt Create() => new();

    public static WorkDateExpiredResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not WorkDateExpiredResult)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredResult)}を渡してください",
                nameof(validationResult));

        return Create();
    }
}
