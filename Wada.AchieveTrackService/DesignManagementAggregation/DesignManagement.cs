namespace Wada.AchieveTrackService.DesignManagementAggregation;

public record class DesignManagement
{
    private DesignManagement(uint ownCompanyNumber, DateTime? startDate, string? designLead)
    {
        OwnCompanyNumber = ownCompanyNumber;
        StartDate = startDate;
        DesignLead = designLead;
    }

    public static DesignManagement Create(uint ownCompanyNumber, DateTime? startDate, string? designLead)
        => new(ownCompanyNumber, startDate, designLead);

    public static DesignManagement Reconstruct(uint ownCompanyNumber, DateTime? startDate, string? designLead)
        => new(ownCompanyNumber, startDate, designLead);

    /// <summary>
    /// 自社NO
    /// </summary>
    public uint OwnCompanyNumber { get; }

    /// <summary>
    /// 着手実績日
    /// </summary>
    public DateTime? StartDate { get; }

    /// <summary>
    /// 設計責任者
    /// </summary>
    public string? DesignLead { get; }
}

public class TestDesignManagementFactory
{
    public static DesignManagement Create(uint ownCompanyNumber = 2002010040u,
                                          DateTime? startDate = default,
                                          string? designLead = default)
    {
        startDate ??= new DateTime(2002, 4, 10);
        return DesignManagement.Reconstruct(ownCompanyNumber, startDate, designLead);
    }
}