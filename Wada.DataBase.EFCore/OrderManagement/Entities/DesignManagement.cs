using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M設計管理")]
public record class DesignManagement
{
    [Key, Required, Column("自社NO")]
    public int OwnCompanyNumber { get; set; } 

    [Column("着手実績日")]
    public DateTime? StartDate { get; set; }

    [Column("設計責任者")]
    public string? DesignLead { get; set; } 
}
