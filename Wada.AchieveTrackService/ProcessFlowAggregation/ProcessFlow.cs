namespace Wada.AchieveTrackService.ProcessFlowAggregation;

public record class ProcessFlow
{
    private ProcessFlow(uint id, string name)
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

    public static ProcessFlow Reconstruct(uint id, string name) => new(id, name);
}

public class TestProcessFlowFactory
{
    public static ProcessFlow Create(uint id = default, string name = "NC")
        => ProcessFlow.Reconstruct(id, name);
}