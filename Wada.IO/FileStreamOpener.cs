
namespace Wada.IO;

public class FileStreamOpener : IFileStreamOpener
{
    [Logging]
    public async Task<Stream> OpenOrCreateAsync(string path)
    {
        try
        {
            return await OpenFileStreamAsync(path);
        }
        catch (IOException ex)
        {
            string msg = $"ファイルが使用中です ファイルパス: {path}";
            throw new FileStreamOpenerException(msg, ex);
        }
    }

    [Logging]
    private static Task<FileStream> OpenFileStreamAsync(string filePath)
    {
        FileInfo fileInfo = new(filePath);
        // ファイルの存在を確認
        if (!fileInfo.Exists)
        {
            if (fileInfo.Directory != null
                && !fileInfo.Directory.Exists)
            {
                fileInfo.Directory.Create();
            }
        }

        return Task.Run(
            () => File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
    }
}