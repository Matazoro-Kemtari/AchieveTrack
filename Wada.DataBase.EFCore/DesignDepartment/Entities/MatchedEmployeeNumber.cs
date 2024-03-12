using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.DesignDepartment.Entities;

[Table("MatchedEmployeeNumbers")]
public record class MatchedEmployeeNumber
{
    [Key, Required]
    public int EmployeeNumbers { get; set; } 

    [Required]
    public int AttendancePersonalCode { get; set; } 
}
