using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M部品明細台帳")]
public record class PartDetailLedger
{
    [Key, Required, Column("明細NO")]
    public int DetailNumber { get; set; }

    [Required, Column("自社NO")]
    public int OwnCompanyNumber { get; set; }

    [Column("品番１")]
    public string? PartNumber1st { get; set; }

    [Column("品番２")]
    public string? PartNumber2nd { get; set; }

    [Column("品番３")]
    public string? PartNumber3rd { get; set; }

    [Column("品番４")]
    public string? PartNumber4th { get; set; }
}
