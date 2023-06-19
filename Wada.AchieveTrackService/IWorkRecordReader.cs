using Wada.AchieveTrackService.AchieveTrackReader;

namespace Wada.AchieveTrackService;

public interface IWorkRecordReader
{
    public Task<IEnumerable<WorkRecord>> ReadWorkRecordsAsync(Stream stream);
}
