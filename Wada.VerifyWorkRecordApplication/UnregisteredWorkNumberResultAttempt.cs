using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class UnregisteredWorkNumberResultAttempt : UnregisteredWorkNumberResult, IValidationResultAttempt
{
    private UnregisteredWorkNumberResultAttempt() { }

    private static new UnregisteredWorkNumberResultAttempt Create() => new();

    public static UnregisteredWorkNumberResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not UnregisteredWorkNumberResult)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkNumberResult)}を渡してください",
                nameof(validationResult));

        return Create();
    }
}
