using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("S記号")]
public record class Symbol
{
    [Key, Required, Column("記号ID")]
    public int Id { get; set; }

    [Column("記号")]
    public string? Character { get; set; }
}
