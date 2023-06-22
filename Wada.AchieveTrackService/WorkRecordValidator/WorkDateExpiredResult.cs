namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業日が完成を過ぎている結果
/// </summary>
public record class WorkDateExpiredResult : IValidationResult
{
    protected WorkDateExpiredResult() { }

    public static WorkDateExpiredResult Create() => new();

    public string Message => "作業日が完成を過ぎています";
}
