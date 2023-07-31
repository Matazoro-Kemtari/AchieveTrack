using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 設計管理に未登録の作業番号の結果
/// </summary>
public record class UnregisteredWorkNumberResult : IValidationResult
{
    protected UnregisteredWorkNumberResult(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public static UnregisteredWorkNumberResult Create(WorkingNumber workingNumber, string jigCode, string note) => new(workingNumber, jigCode, note);

    public string Message => "設計管理に未登録の作業番号です";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}

public class TestUnregisteredWorkNumberResultFactory
{
    public static UnregisteredWorkNumberResult Create(WorkingNumber? workingNumber = default,
                                                      string jigCode = "11A",
                                                      string note = "特記事項")
    {
        workingNumber ??= TestWorkingNumberFactory.Create();
        return UnregisteredWorkNumberResult.Create(workingNumber, jigCode, note);
    }
}
