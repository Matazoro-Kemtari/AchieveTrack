using Wada.AchieveTrackService.WorkRecordReader;
using Wada.AchieveTrackService.WorkRecordValidator;

namespace Wada.AchieveTrackService;

public interface IWorkRecordValidator
{
    /// <summary>
    /// 日報の内容を検証する
    /// </summary>
    /// <param name="workRecords"></param>
    /// <returns></returns>
    public Task<IEnumerable<IEnumerable<IValidationResult>>> ValidateWorkRecordsAsync(IEnumerable<WorkRecord> workRecords);
}
