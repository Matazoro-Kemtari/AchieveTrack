using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("S社員")]
public record class Employee
{
    [Key, Required, Column("社員NO")]
    public int EmployeeNumber { get; set; }

    [Required, Column("氏名")]
    public string Name { get; set; } = string.Empty;

    [Column("部署ID")]
    public int? DepartmentID { get; set; }

    [Column("実績工程ID")]
    public int? ProcessFlowId { get; set; }
}
