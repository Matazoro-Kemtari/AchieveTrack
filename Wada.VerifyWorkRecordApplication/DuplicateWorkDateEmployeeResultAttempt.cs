using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class DuplicateWorkDateEmployeeResultAttempt : DuplicateWorkDateEmployeeResult, IValidationResultAttempt
{
    private DuplicateWorkDateEmployeeResultAttempt() { }

    private static new DuplicateWorkDateEmployeeResultAttempt Create() => new();

    public static DuplicateWorkDateEmployeeResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not DuplicateWorkDateEmployeeResult)
            throw new ArgumentException(
                $"引数には{nameof(DuplicateWorkDateEmployeeResult)}を渡してください",
                nameof(validationResult));

        return Create();
    }
}