using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wada.DataBase.EFCore.OrderManagement.Entities;

[Table("M実績台帳明細")]
public record class AchievementDetail
{
    // Contextで複合Keyの設定をしている

    [Required, Column("実績ID")]
    public int AchievementLedgerId { get; set; } 

    [Required, Column("自社NO")]
    public int OwnCompanyNumber { get; set; }

    [Required, Column("実績工程ID")]
    public int ProcessFlowId { get; set; }

    [Column("目標工数")]
    public double? TargetManHour { get; set; }

    [Column("実績工数")]
    public double? ActualManHour { get; set; }

    // ナビゲーションプロパティ
    public AchievementLedger? AchievementLedger { get; set; }
}
