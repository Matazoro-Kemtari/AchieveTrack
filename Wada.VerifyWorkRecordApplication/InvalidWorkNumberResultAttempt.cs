using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class InvalidWorkNumberResultAttempt : InvalidWorkNumberResult, IValidationResultAttempt
{
    protected InvalidWorkNumberResultAttempt(WorkingNumber workingNumber, string note)
        : base(workingNumber, note)
    { }

    private static new InvalidWorkNumberResultAttempt Create(WorkingNumber workingNumber, string note)
        => new(workingNumber, note);

    public static InvalidWorkNumberResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not InvalidWorkNumberResult)
            throw new ArgumentException(
                $"引数には{nameof(InvalidWorkNumberResult)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.Note);
    }
}
