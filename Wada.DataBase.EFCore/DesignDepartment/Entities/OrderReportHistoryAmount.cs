using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.DesignDepartment.Entities;

[Table("OrderReportHistoryAmounts")]
public class OrderReportHistoryAmount
{
    [Key, Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public int OrderYear { get; set; }

    [Required]
    public int OrderMonth { get; set; }

    [Required]
    public int OwnCompanyNumber { get; set; }

    [Required]
    public decimal ProspectiveSalesOrderAmount { get; set; }

    [Required]
    public decimal SalesOrderAmount { get; set; }

    [Required]
    public string OrderReportHistoryId { get; set; } = string.Empty;

    // ナビゲーションプロパティ
    public OrderReportHistory? OrderReportHistory { get; set; }
}
