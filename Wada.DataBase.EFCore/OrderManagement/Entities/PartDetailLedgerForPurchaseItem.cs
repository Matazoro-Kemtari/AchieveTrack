using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M部品明細台帳購入品")]
public record class PartDetailLedgerForPurchaseItem
{
    [Key, Required, Column("明細NO")]
    public int DetailNumber { get; set; }

    [Column("金額発注")]
    public decimal? OrderAmount { get; set; }

    [Column("発注日")]
    public DateTime? OrderDate { get; set; }

    [Column("金額入荷")]
    public decimal? ArrivalAmount { get; set; }
}
