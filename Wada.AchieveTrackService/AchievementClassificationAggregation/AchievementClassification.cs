namespace Wada.AchieveTrackService.AchievementClassificationAggregation;

public record class AchievementClassification
{
    private AchievementClassification(uint id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// 実績工程ID
    /// </summary>
    public uint Id { get; }

    /// <summary>
    /// 実績工程
    /// </summary>
    public string Name { get; }

    public static AchievementClassification Reconstruct(uint id, string name) => new(id, name);
}

public class TestAchievementClassificationFactory
{
    public static AchievementClassification Create(uint id = default, string name = "NC")
        => AchievementClassification.Reconstruct(id, name);
}