using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

public interface IValidationResult
{
    string Message { get; }

    public WorkingNumber WorkingNumber { get; }

    public string Note { get; }
}
