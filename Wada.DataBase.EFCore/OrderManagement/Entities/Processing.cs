using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("S加工")]
public record class Processing
{
    [Key, Required, Column("加工ID")]
    public int Id { get; set; }

    [Column("加工名")]
    public string? Name { get; set; }
}
