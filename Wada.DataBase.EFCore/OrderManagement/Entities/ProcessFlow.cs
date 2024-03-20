using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("S実績工程")]
public record class ProcessFlow
{
    [Key, Required, Column("実績工程ID")]
    public int Id { get; set; }

    [Required, Column("実績工程")]
    public string Name { get; set; } = string.Empty;
}
