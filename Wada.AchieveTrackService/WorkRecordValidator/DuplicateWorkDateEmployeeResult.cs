namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業日と社員NOの組み合わせが既に存在する結果
/// </summary>
public record class DuplicateWorkDateEmployeeResult : IValidationResult
{
    protected DuplicateWorkDateEmployeeResult() { }

    public static DuplicateWorkDateEmployeeResult Create() => new();

    public string Message => "この作業日と社員番号の組み合わせが 実績処理で既に存在します";
}
