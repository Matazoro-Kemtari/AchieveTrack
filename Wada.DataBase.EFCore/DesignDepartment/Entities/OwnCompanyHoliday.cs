using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.DesignDepartment.Entities;

[Table("OwnCompanyHolidays")]
public record class OwnCompanyHoliday
{
    [Key, Required]
    public string CalendarGroupId { get; set; } = string.Empty;

    [Key, Required]
    public DateTime HolidayDate { get; set; } 

    [Required]
    public bool LegalHoliday { get; set; } 
}
