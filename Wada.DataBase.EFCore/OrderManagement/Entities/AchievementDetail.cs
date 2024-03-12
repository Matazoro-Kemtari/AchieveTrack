using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M実績台帳明細")]
public record class AchievementDetail
{
    [Key, Required, Column("実績ID")]
    public int AchievementLedgerId { get; set; } 

    [Key, Required, Column("自社NO")]
    public int OwnCompanyNumber { get; set; }

    [Key, Required, Column("実績工程ID")]
    public int AchievementProcessId { get; set; }

    [Column("目標工数")]
    public double? TargetManHour { get; set; }

    [Column("実績工数")]
    public double? ActualManHour { get; set; }

    // ナビゲーションプロパティ
    public AchievementLedger? AchievementLedger { get; set; }
}
