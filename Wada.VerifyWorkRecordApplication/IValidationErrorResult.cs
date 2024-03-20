namespace Wada.VerifyAchievementRecordContentApplication;

public interface IValidationErrorResult
{
    string Message { get; }

    public string WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}