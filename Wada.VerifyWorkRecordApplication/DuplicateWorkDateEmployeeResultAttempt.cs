using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class DuplicateWorkDateEmployeeResultAttempt : DuplicateWorkDateEmployeeResult, IValidationResultAttempt
{
    protected DuplicateWorkDateEmployeeResultAttempt(WorkingNumber workingNumber, string jigCode, string note)
        : base(workingNumber, jigCode, note)
    { }

    private static new DuplicateWorkDateEmployeeResultAttempt Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static DuplicateWorkDateEmployeeResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not DuplicateWorkDateEmployeeResult)
            throw new ArgumentException(
                $"引数には{nameof(DuplicateWorkDateEmployeeResult)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }
}