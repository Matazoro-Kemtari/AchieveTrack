using Wada.AchieveTrackService.DesignManagementAggregation;

namespace Wada.AchieveTrackService.DesignManagementWriter;

public class DesignManagementWriter : IDesignManagementWriter
{
    private readonly IDesignManagementRepository _designManagementRepository;

    public DesignManagementWriter(IDesignManagementRepository designManagementRepository)
    {
        _designManagementRepository = designManagementRepository;
    }

    public int Add(uint ownCompanyNumber)
    {
        try
        {
            // 既存レコードがないか確認する
            _ = _designManagementRepository.FindByOwnCompanyNumber(ownCompanyNumber);
            return default;
        }
        catch (DesignManagementAggregationException)
        { /* 既存レコードなし */ }

        try
        {
            var model = DesignManagement.Create(
                ownCompanyNumber, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), "岡田");
            return _designManagementRepository.Add(model);
        }
        catch (DesignManagementAggregationException ex)
        {
            throw new DesignManagementWriterException(ex.Message, ex);
        }
    }
}
