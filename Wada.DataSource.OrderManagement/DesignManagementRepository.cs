using Wada.AchieveTrackService;
using Wada.AchieveTrackService.DesignManagementAggregation;
using Wada.AOP.Logging;

namespace Wada.DataSource.OrderManagement;

public class DesignManagementRepository : IDesignManagementRepository
{
    private readonly Data.OrderManagement.Models.IDesignManagementRepository _designManagementRepository;

    public DesignManagementRepository(Data.OrderManagement.Models.IDesignManagementRepository designManagementRepository)
    {
        _designManagementRepository = designManagementRepository;
    }

    [Logging]
    public int Add(DesignManagement designManagement)
    {
        try
        {
            return _designManagementRepository.Add(designManagement.Convert());
        }
        catch (Data.OrderManagement.Models.DesignManagementAggregation.DesignManagementAggregationException ex)
        {
            throw new DesignManagementAggregationException(ex.Message, ex);
        }
    }

    [Logging]
    public DesignManagement FindByOwnCompanyNumber(uint ownCompanyNumber)
    {
        try
        {
            var designManagement = _designManagementRepository.FindByOwnCompanyNumber(ownCompanyNumber);
            return DesignManagement.Parse(designManagement);
        }
        catch (Data.OrderManagement.Models.DesignManagementAggregation.DesignManagementAggregationException ex)
        {
            throw new DesignManagementAggregationException(ex.Message, ex);
        }
    }

    [Logging]
    public async Task<DesignManagement> FindByOwnCompanyNumberAsync(uint ownCompanyNumber)
    {
        try
        {
            var designManagement = await _designManagementRepository.FindByOwnCompanyNumberAsync(ownCompanyNumber);
            return DesignManagement.Parse(designManagement);
        }
        catch (Data.OrderManagement.Models.DesignManagementAggregation.DesignManagementAggregationException ex)
        {
            throw new DesignManagementAggregationException(ex.Message, ex);
        }
    }
}
