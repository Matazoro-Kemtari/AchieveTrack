using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.DesignDepartment.Entities;

[Table("Achievements")]
public record class Achievement
{
    [Key, Required]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string WorkingNumber { get; set; } = string.Empty;

    public string? Det { get; set; }

    [Required]
    public string AchievementProcess { get; set; } = string.Empty;

    [Required]
    public string MajorWorkingClassification { get; set; } = string.Empty;

    public string? MiddleWorkingClassification { get; set; }

    [Required]
    public decimal ManHour { get; set; }

    public string? Note { get; set; }

    [Required]
    public string AttendanceId { get; set; } = string.Empty;

    // ナビゲーションプロパティ
    public Attendance? Attendance { get; set; }
}
