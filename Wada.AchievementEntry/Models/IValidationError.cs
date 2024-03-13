namespace Wada.AchievementEntry.Models;

public interface IValidationError
{
    string Message { get; }

    public string WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}
