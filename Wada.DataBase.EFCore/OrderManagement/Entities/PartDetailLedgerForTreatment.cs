using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M部品明細台帳処理")]
public record class PartDetailLedgerForTreatment
{
    [Key, Required, Column("明細NO")]
    public int DetailNumber { get; set; }

    [Column("納期１")]
    public DateTime? DeliveryDate1st { get; set; }

    [Column("金額入荷１")]
    public decimal? ArrivalAmount1st { get; set; }

    [Column("納期２")]
    public DateTime? DeliveryDate2nd { get; set; }

    [Column("金額入荷２")]
    public decimal? ArrivalAmount2nd { get; set; }
}