namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業台帳にない作業NOの結果
/// </summary>
public record class InvalidWorkNumberResult : IValidationResult
{
    protected InvalidWorkNumberResult() { }

    public static InvalidWorkNumberResult Create() => new();

    public string Message => "作業台帳にない作業NOです";
}
