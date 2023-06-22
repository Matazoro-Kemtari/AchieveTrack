namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 成功
/// </summary>
public record class ValidationSuccessResult : IValidationResult
{
    protected ValidationSuccessResult() { }

    public static ValidationSuccessResult Create() => new();

    public string Message => "Validation succeeded.";
}
