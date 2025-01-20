namespace Wada.AchievementEntry.Models;

public interface IValidationError
{
    string Message { get; }

    public string WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }
}
