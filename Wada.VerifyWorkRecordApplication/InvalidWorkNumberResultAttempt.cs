using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class InvalidWorkNumberResultAttempt : InvalidWorkNumberResult, IValidationResultAttempt
{
    protected InvalidWorkNumberResultAttempt() { }

    private static new InvalidWorkNumberResultAttempt Create() => new();

    public static InvalidWorkNumberResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not InvalidWorkNumberResult)
            throw new ArgumentException(
                $"引数には{nameof(InvalidWorkNumberResult)}を渡してください",
                nameof(validationResult));

        return Create();
    }
}
