using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.DesignDepartment.Entities;

[Table("DepartmentCompanyHolidays")]
public record class DepartmentCompanyHoliday
{
    [Key, Required]
    public int DepartmentId { get; set; }

    [Required]
    public string CalendarGroupId { get; set; } = string.Empty;
}
