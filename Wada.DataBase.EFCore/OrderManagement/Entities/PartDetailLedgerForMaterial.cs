using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M部品明細台帳材料")]
public record class PartDetailLedgerForMaterial
{
    [Key, Required, Column("明細NO")]
    public int DetailNumber { get; set; }

    [Column("金額発注")]
    public decimal? OrderAmount { get; set; }

    [Column("管理NO")]
    public string? ManagementNumber { get; set; }

    [Column("金額入荷")]
    public decimal? ArrivalAmount { get; set; }
}