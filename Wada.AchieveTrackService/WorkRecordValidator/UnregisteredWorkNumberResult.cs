namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 設計管理に未登録の作業番号の結果
/// </summary>
public record class UnregisteredWorkNumberResult : IValidationResult
{
    protected UnregisteredWorkNumberResult() { }

    public static UnregisteredWorkNumberResult Create() => new();

    public string Message => "設計管理に未登録の作業番号です";
}
