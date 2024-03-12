using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M実績台帳ヘダー")]
public record class AchievementLedger
{
    [Key, Required, Column("実績ID")]
    public int Id { get; set; } 

    [Required, Column("作業日")]
    public DateTime WorkingDate { get; set; }

    [Required, Column("社員NO")]
    public int EmployeeNumber { get; set; }

    [Column("部署ID")]
    public int? DepartmentID { get; set; }

    [Column("実働時間")]
    public double? ActualWorkTime { get; set; }

    // ナビゲーションプロパティ
    public ICollection<AchievementDetail> AchievementDetails { get; set; } = new List<AchievementDetail>();
}
