using Wada.AchieveTrackService;
using Wada.AOP.Logging;
using Wada.IO;

namespace Wada.ReadWorkRecordApplication;

public interface IReadAchieveTrackUseCase
{
    /// <summary>
    /// 日報エクセルを読み込む
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    Task<IEnumerable<WorkRecordResult>> ExecuteAsync(IEnumerable<string> paths);
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

    [Logging]
    public async Task<IEnumerable<WorkRecordResult>> ExecuteAsync(IEnumerable<string> paths)
    {
        try
        {
            var results = await Task.WhenAll(
                paths.Select(async path =>
                {
                    // エクセルを開く
                    using var stream = _fileStreamOpener.Open(path);

                    if (stream.Length == 0)
                        throw new ReadAchieveTrackUseCaseException(
                            $"ファイルサイズが不正です パス: {path}");

                    // 日報オブジェクトを作成する
                    return await _workRecordReader.ReadWorkRecordsAsync(stream);
                }));

            return results.SelectMany(x => x)
                          .Select(x => WorkRecordResult.Parse(x));
        }
        catch (FileStreamOpenerException ex)
        {
            throw new AchieveTrackIOException(ex.Message, ex);
        }
        catch (DomainException ex)
        {
            throw new ReadAchieveTrackUseCaseException(ex.Message, ex);
        }
    }
}
