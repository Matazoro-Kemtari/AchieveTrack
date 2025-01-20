using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業日と社員NOの組み合わせが既に存在する結果
/// </summary>
public record class DuplicateWorkDateEmployeeError : IValidationError
{
    protected DuplicateWorkDateEmployeeError(WorkOrderId workOrderId, string jigCode, string note)
    {
        WorkOrderId = workOrderId;
        JigCode = jigCode;
        Note = note;
    }

    public static DuplicateWorkDateEmployeeError Create(WorkOrderId workOrderId, string jigCode, string note) => new(workOrderId, jigCode, note);

    public string Message => "この作業日と社員番号の組み合わせが 実績処理で既に存在します";

    public WorkOrderId WorkOrderId { get; }

    public string JigCode { get; }

    public string Note { get; }
}

public class TestDuplicateWorkDateEmployeeResultFactory
{
    public static DuplicateWorkDateEmployeeError Create(WorkOrderId? workOrderId = default,
                                                         string jigCode = "11A",
                                                         string note = "特記事項")
    {
        workOrderId ??= TestWorkOrderIdFactory.Create();
        return DuplicateWorkDateEmployeeError.Create(workOrderId, jigCode, note);
    }
}
