using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M作業台帳")]
public record class WorkingLedger
{
    [Key, Required, Column("自社NO")]
    public int OwnCompanyNumber { get; set; }

    [Required, Column("作業NO")]
    public string WorkingNumber { get; set; } = string.Empty;

    [Required, Column("得意先ID")]
    public int ClientID { get; set; }

    [Column("納期")]
    public DateTime? DeliveryDate { get; set; }

    [Column("ToolNo")]
    public string? ToolNo { get; set; }

    [Column("コード")]
    public string? JigCode { get; set; }

    [Column("状態")]
    public string? OrderStatus { get; set; }

    [Column("完成日")]
    public DateTime? CompletionDate { get; set; }

    [Column("請求日")]
    public DateTime? InvoiceDate {  get; set; }

    [Column("見積金額")]
    public decimal? QuoteAmount { get; set; }

    [Column("請求金額")]
    public decimal? InvoiceAmount { get; set; }

    [Column("納品日")]
    public DateTime? DeliveredDate { get; set; }
}
