using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class ValidationSuccessResultAttempt : ValidationSuccessResult, IValidationResultAttempt
{
    private ValidationSuccessResultAttempt() { }

    private static new ValidationSuccessResultAttempt Create() => new();

    public static ValidationSuccessResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not ValidationSuccessResult)
            throw new ArgumentException(
                $"引数には{nameof(ValidationSuccessResult)}を渡してください",
                nameof(validationResult));

        return Create();
    }
}
