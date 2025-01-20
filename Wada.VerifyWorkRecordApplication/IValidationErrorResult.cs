namespace Wada.VerifyAchievementRecordContentApplication;

public interface IValidationErrorResult
{
    string Message { get; }

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }
}