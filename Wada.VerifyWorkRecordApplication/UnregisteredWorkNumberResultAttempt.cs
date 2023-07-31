using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class UnregisteredWorkNumberResultAttempt : UnregisteredWorkNumberResult, IValidationResultAttempt
{
    protected UnregisteredWorkNumberResultAttempt(WorkingNumber workingNumber, string jigCode, string note)
        : base(workingNumber, jigCode, note)
    { }

    private static new UnregisteredWorkNumberResultAttempt Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static UnregisteredWorkNumberResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not UnregisteredWorkNumberResult)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkNumberResult)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }
}
