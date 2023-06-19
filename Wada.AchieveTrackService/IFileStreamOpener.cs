namespace Wada.AchieveTrackService;

public interface IFileStreamOpener
{
    /// <summary>
    /// ファイルストリームを開く
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task<Stream> OpenAsync(string path);
}
