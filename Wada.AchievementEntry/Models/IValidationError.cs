using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchievementEntry.Models;

public interface IValidationError
{
    string Message { get; }

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}
