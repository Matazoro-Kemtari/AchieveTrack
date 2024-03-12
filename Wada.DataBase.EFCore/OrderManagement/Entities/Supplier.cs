using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("S外注先")]
public record class Supplier
{
    [Key, Required, Column("外注先ID")]
    public int Id { get; set; }

    [Column("外注先名")]
    public string? Name { get; set; }
}
