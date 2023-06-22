namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 設計管理に未登録の作業NOの結果
/// </summary>
public record class UnregisteredWorkNumberResult : IValidationResult
{
    protected UnregisteredWorkNumberResult() { }

    public static UnregisteredWorkNumberResult Create() => new();

    public string Message => "設計管理に未登録の作業NOです";
}
