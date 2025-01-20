using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業日が完成を過ぎている結果
/// </summary>
public record class WorkDateExpiredError : IValidationError
{
    protected WorkDateExpiredError(WorkOrderId workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public static WorkDateExpiredError Create(WorkOrderId workOrderId, string jigCode, string note) => new(workOrderId, jigCode, note);

    public string Message => "作業日が完成を過ぎています";

    public WorkOrderId WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }
}

public class TestWorkDateExpiredResultFactory
{
    public static WorkDateExpiredError Create(WorkOrderId? workOrderId = default,
                                               string jigCode = "11A",
                                               string note = "特記事項")
    {
        workOrderId ??= TestWorkOrderIdFactory.Create();
        return WorkDateExpiredError.Create(workOrderId, jigCode, note);
    }
}
