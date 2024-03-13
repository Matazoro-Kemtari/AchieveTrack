using Wada.AchieveTrackService.ValueObjects;

namespace Wada.VerifyAchievementRecordContentApplication;

public interface IValidationErrorResult
{
    string Message { get; }

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}