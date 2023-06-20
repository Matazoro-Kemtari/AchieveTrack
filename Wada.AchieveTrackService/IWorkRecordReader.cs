using Wada.AchieveTrackService.AchieveTrackReader;

namespace Wada.AchieveTrackService;

public interface IWorkRecordReader
{
    /// <summary>
    /// 日報を読み込む
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="DomainException"></exception>
    public Task<IEnumerable<WorkRecord>> ReadWorkRecordsAsync(Stream stream);
}
