using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業台帳にない作業番号の結果
/// </summary>
public record class InvalidWorkOrderIdError : IValidationError
{
    protected InvalidWorkOrderIdError(WorkOrderId workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public static InvalidWorkOrderIdError Create(WorkOrderId workOrderId, string jigCode, string note) => new(workOrderId, jigCode, note);

    public string Message => "作業台帳にない作業番号です";

    public WorkOrderId WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }
}

public class TestInvalidWorkNumberResultFactory
{
    public static InvalidWorkOrderIdError Create(WorkOrderId? workOrderId = default,
                                                 string jigCode = "11A",
                                                 string note = "特記事項")
    {
        workOrderId ??= TestWorkOrderIdFactory.Create();
        return InvalidWorkOrderIdError.Create(workOrderId, jigCode, note);
    }
}
