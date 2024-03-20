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

    [Column("完成日")]
    public DateTime? CompletionDate { get; set; }
}
