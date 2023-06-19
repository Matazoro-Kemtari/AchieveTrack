using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text;
using Wada.ManHourRecordService;

namespace Wada.IO.Tests;

[TestClass()]
public class FileStreamOpenerTests
{
    [TestMethod()]
    public async Task 正常系_既存のファイルを開くこと()
    {
        // given
        const string testDataName = "Wada.IOTests.Resources.RandomData.txt";
        if (File.Exists(testDataName))
            File.Delete(testDataName);

        // when
        var expected = string.Empty;
        // アセンブリに埋め込まれているリソース"RandomData.txt"のStreamを取得する
        var assembly = Assembly.GetExecutingAssembly();
        using (var resurceStream = assembly.GetManifestResourceStream(testDataName))
        {
            using var resurceReader = new StreamReader(resurceStream!);
            expected = resurceReader.ReadToEnd();
            using var write = File.Create(testDataName);

            byte[] bytes = new UTF8Encoding(true).GetBytes(expected);
            write.Write(bytes, 0, bytes.Length);
        }
        IFileStreamOpener streamOpener = new FileStreamOpener();
        var actual = string.Empty;
        using (var stream = await streamOpener.OpenOrCreateAsync(testDataName))
        using (var reader = new StreamReader(stream!))
            actual = reader.ReadToEnd();

        // then
        Assert.AreEqual(expected, actual);

        File.Delete(testDataName);
    }

    [TestMethod()]
    public async Task 正常系_新規ファイルを開くこと()
    {
        // given
        const string testDataName = "Wada.IOTests.Resources.RandomData.txt";
        if (File.Exists(testDataName))
            File.Delete(testDataName);

        // when
        IFileStreamOpener streamOpener = new FileStreamOpener();
        using var stream = await streamOpener.OpenOrCreateAsync(testDataName);

        // then
        Assert.AreEqual(0, stream.Length);
        stream.Close();
        Assert.IsTrue(File.Exists(testDataName));

        File.Delete(testDataName);
    }

    [TestMethod()]
    public async Task 異常系_他のプロセスがファイルをロックしているファイルを開こうとすると例外を返すこと()
    {
        // given
        const string testDataName = "Wada.IOTests.Resources.RandomData.txt";
        using var fileStream = File.Open(testDataName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);

        try
        {
            // when
            IFileStreamOpener streamOpener = new FileStreamOpener();
            Task<Stream> target() => streamOpener.OpenOrCreateAsync(testDataName);

            // then
            var ex = await Assert.ThrowsExceptionAsync<FileStreamOpenerException>(target);
            var message = $"ファイルが使用中です ファイルパス: {testDataName}";
            Assert.AreEqual(message, ex.Message);
        }
        finally
        {
            fileStream.Dispose();
            File.Delete(testDataName);
        }
    }
}