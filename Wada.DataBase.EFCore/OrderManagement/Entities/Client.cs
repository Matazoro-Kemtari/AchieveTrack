using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("S得意先")]
public record class Client
{
    [Key, Required, Column("得意先ID")]
    public int Id { get; set; } 

    [Column("得意先名")]
    public string? Name { get; set; } 

    [Column("得意先名略")]
    public string? ShortName { get; set; } 

    [Column("記号ID")]
    public int? SymbolId { get; set; } 
}
