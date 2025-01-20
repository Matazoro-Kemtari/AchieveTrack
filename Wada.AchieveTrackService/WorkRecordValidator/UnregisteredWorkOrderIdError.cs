using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 設計管理に未登録の作業番号の結果
/// </summary>
public record class UnregisteredWorkOrderIdError : IValidationError
{
    protected UnregisteredWorkOrderIdError(WorkOrderId workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public static UnregisteredWorkOrderIdError Create(WorkOrderId workOrderId, string jigCode, string note) => new(workOrderId, jigCode, note);

    public string Message => "設計管理に未登録の作業番号です";

    public WorkOrderId WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }
}

public class TestUnregisteredWorkOrderIdResultFactory
{
    public static UnregisteredWorkOrderIdError Create(WorkOrderId? workOrderId = default,
                                                      string jigCode = "11A",
                                                      string note = "特記事項")
    {
        workOrderId ??= TestWorkOrderIdFactory.Create();
        return UnregisteredWorkOrderIdError.Create(workOrderId, jigCode, note);
    }
}
