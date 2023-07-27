using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class DuplicateWorkDateEmployeeResultAttempt : DuplicateWorkDateEmployeeResult, IValidationResultAttempt
{
    protected DuplicateWorkDateEmployeeResultAttempt(WorkingNumber workingNumber, string note)
        : base(workingNumber, note)
    { }

    private static new DuplicateWorkDateEmployeeResultAttempt Create(WorkingNumber workingNumber, string note)
        => new(workingNumber, note);

    public static DuplicateWorkDateEmployeeResultAttempt Parse(IValidationResult validationResult)
    {
        if (validationResult is not DuplicateWorkDateEmployeeResult)
            throw new ArgumentException(
                $"引数には{nameof(DuplicateWorkDateEmployeeResult)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.Note);
    }
}