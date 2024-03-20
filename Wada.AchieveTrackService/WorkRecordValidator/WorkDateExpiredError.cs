using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業日が完成を過ぎている結果
/// </summary>
public record class WorkDateExpiredError : IValidationError
{
    protected WorkDateExpiredError(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public static WorkDateExpiredError Create(WorkingNumber workingNumber, string jigCode, string note) => new(workingNumber, jigCode, note);

    public string Message => "作業日が完成を過ぎています";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}

public class TestWorkDateExpiredResultFactory
{
    public static WorkDateExpiredError Create(WorkingNumber? workingNumber = default,
                                               string jigCode = "11A",
                                               string note = "特記事項")
    {
        workingNumber ??= TestWorkingNumberFactory.Create();
        return WorkDateExpiredError.Create(workingNumber, jigCode, note);
    }
}
