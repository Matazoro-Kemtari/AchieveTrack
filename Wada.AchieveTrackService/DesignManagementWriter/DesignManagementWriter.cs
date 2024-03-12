using Wada.AchieveTrackService.DesignManagementAggregation;
using Wada.AOP.Logging;

namespace Wada.AchieveTrackService.DesignManagementWriter;

public class DesignManagementWriter : IDesignManagementWriter
{
    private readonly IDesignManagementRepository _designManagementRepository;

    public DesignManagementWriter(IDesignManagementRepository designManagementRepository)
    {
        _designManagementRepository = designManagementRepository;
    }

    [Logging]
    public int Add(uint ownCompanyNumber, DateTime workingDate)
    {
        try
        {
            // 既存レコードがないか確認する
            _ = _designManagementRepository.FindByOwnCompanyNumber(ownCompanyNumber);
            return default;
        }
        catch (DesignManagementNotFoundException)
        { /* 既存レコードなし */ }

        try
        {
            var model = DesignManagement.Create(
                ownCompanyNumber, new DateTime(workingDate.Year, workingDate.Month, 1), "岡田");
            return _designManagementRepository.Add(model);
        }
        catch (DesignManagementAggregationException ex)
        {
            throw new DesignManagementWriterException(ex.Message, ex);
        }
    }
}
