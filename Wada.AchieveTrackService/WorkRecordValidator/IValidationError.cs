using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

public interface IValidationError
{
    string Message { get; }

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}
