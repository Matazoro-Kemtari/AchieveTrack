using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業日が完成を過ぎている結果
/// </summary>
public record class WorkDateExpiredResult : IValidationResult
{
    protected WorkDateExpiredResult(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public static WorkDateExpiredResult Create(WorkingNumber workingNumber, string jigCode, string note) => new(workingNumber, jigCode, note);

    public string Message => "作業日が完成を過ぎています";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}

public class TestWorkDateExpiredResultFactory
{
    public static WorkDateExpiredResult Create(WorkingNumber? workingNumber = default,
                                               string jigCode = "11A",
                                               string note = "特記事項")
    {
        workingNumber ??= TestWorkingNumberFactory.Create();
        return WorkDateExpiredResult.Create(workingNumber, jigCode, note);
    }
}
