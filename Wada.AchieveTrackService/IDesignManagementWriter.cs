namespace Wada.AchieveTrackService;

public interface IDesignManagementWriter
{
    /// <summary>
    /// 設計管理に登録する
    /// </summary>
    /// <param name="ownCompanyNumber"></param>
    /// <param name="workingDate"></param>
    /// <returns></returns>
    public int Add(uint ownCompanyNumber, DateTime workingDate);
}
