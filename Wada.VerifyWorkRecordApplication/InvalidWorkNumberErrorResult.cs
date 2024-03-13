using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyAchievementRecordContentApplication;

namespace Wada.VerifyWorkRecordApplication;

public record class InvalidWorkNumberErrorResult : IValidationErrorResult
{
    private InvalidWorkNumberErrorResult(string workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public string Message => "作業台帳にない作業番号です";

    public string WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }

    private static InvalidWorkNumberErrorResult Create(string workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static InvalidWorkNumberErrorResult Parse(IValidationError validationResult)
    {
        if (validationResult is not InvalidWorkNumberError)
            throw new ArgumentException(
                $"引数には{nameof(InvalidWorkNumberError)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber.Value, validationResult.JigCode, validationResult.Note);
    }
}
