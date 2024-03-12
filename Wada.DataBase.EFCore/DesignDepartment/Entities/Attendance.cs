using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.DesignDepartment.Entities;

[Table("Attendances")]
public record class Attendance
{
    [Key, Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public int EmployeeNumber { get; set; }

    [Required]
    public DateTime AchievementDate { get; set; }

    public TimeSpan? StartTime { get; set; }

    public string? DayOffClassification { get; set; }

    public string Department { get; set; } = string.Empty;

    // ナビゲーションプロパティ
    public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();
}
