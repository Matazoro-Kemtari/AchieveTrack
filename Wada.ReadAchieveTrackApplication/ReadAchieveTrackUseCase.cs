using Wada.AchieveTrackService;

namespace Wada.ReadAchieveTrackApplication;

public interface IReadAchieveTrackUseCase
{
    /// <summary>
    /// 日報エクセルを読み込む
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    Task<IEnumerable<WorkRecordAttempt>> ExecuteAsync(IEnumerable<string> paths);
}

public class ReadAchieveTrackUseCase : IReadAchieveTrackUseCase
{
    private readonly IFileStreamOpener _fileStreamOpener;
    private readonly IWorkRecordReader _workRecordReader;

    public ReadAchieveTrackUseCase(IFileStreamOpener fileStreamOpener, IWorkRecordReader workRecordReader)
    {
        _fileStreamOpener = fileStreamOpener;
        _workRecordReader = workRecordReader;
    }

    public async Task<IEnumerable<WorkRecordAttempt>> ExecuteAsync(IEnumerable<string> paths)
    {
        var results = await Task.WhenAll(
            paths.Select(async path =>
            {
                // エクセルを開く
                var stream = await _fileStreamOpener.OpenAsync(path);

                // 日報オブジェクトを作成する
                return await _workRecordReader.ReadWorkRecordsAsync(stream);
            }));

        return results.SelectMany(x => x)
                      .Select(x => WorkRecordAttempt.Parse(x));
    }
}
