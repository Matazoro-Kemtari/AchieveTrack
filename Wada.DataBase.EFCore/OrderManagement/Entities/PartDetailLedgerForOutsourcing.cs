using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M部品明細台帳外注")]
public record class PartDetailLedgerForOutsourcing
{
    [Key, Required, Column("部品明細台帳外注ID")]
    public int Id { get; set; }

    [Required, Column("明細NO")]
    public int DetailNumber { get; set; }

    [Column("外注先ID")]
    public int? SupplierId { get; set; }

    [Column("加工ID")]
    public int? ProcessingId { get; set; }

    [Column("工数")]
    public double? WorkHours { get; set; }

    [Column("単価")]
    public decimal? UnitPrice { get; set; }

    [Column("金額")]
    public decimal? Amount { get; set; }

    [Column("管理NO")]
    public string? ManagementNumber { get; set; }

    [Column("発注日")]
    public DateTime? OrderDate { get; set; }

    [Column("納期")]
    public DateTime? DeliveryDate { get; set; }

    [Column("入荷日")]
    public DateTime? ArrivalDate { get; set; }

    [Column("金額入荷")]
    public decimal? ArrivalAmount { get; set; }

    [Column("確認日")]
    public DateTime? AcceptanceConfirmationDate { get; set; }
}
