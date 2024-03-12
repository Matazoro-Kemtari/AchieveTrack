using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.DesignDepartment.Entities;

[Table("OrderReportHistories")]
public class OrderReportHistory
{
    [Key, Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public DateTime CheckingDate { get; set; }

    // ナビゲーションプロパティ
    public ICollection<OrderReportHistoryAmount> OrderReportHistoriesAmounts { get; set; } = new List<OrderReportHistoryAmount>();
}
