using Wada.AchieveTrackService.DesignManagementAggregation;

namespace Wada.AchieveTrackService;

public interface IDesignManagementRepository
{
    /// <summary>
    /// 設計管理を取得する
    /// </summary>
    /// <returns></returns>
    IEnumerable<DesignManagement> FindAll();

    /// <summary>
    /// 自社NOの設計管理を検索する
    /// </summary>
    /// <param name="ownCompanyNumber">自社NO</param>
    /// <returns></returns>
    DesignManagement FindByOwnCompanyNumber(uint ownCompanyNumber);

    /// <summary>
    /// 自社NOの設計管理を検索する
    /// </summary>
    /// <param name="ownCompanyNumber">自社NO</param>
    /// <returns></returns>
    Task<DesignManagement> FindByOwnCompanyNumberAsync(uint ownCompanyNumber);
    
    /// <summary>
    /// 設計管理に追加する
    /// </summary>
    /// <param name="designManagement"></param>
    /// <returns></returns>
    int Add(DesignManagement designManagement);
}
