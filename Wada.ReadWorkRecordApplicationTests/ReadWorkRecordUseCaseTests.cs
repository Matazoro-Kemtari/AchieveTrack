using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordReader;
using Wada.IO;

namespace Wada.ReadWorkRecordApplication.Tests
{
    [TestClass()]
    public class ReadAchieveTrackUseCaseTests
    {
        [TestMethod()]
        public async Task 正常系_作業日報が取得できること()
        {
            // given
            Mock<IFileStreamOpener> streamMock = new();
            var paths = new string[]
            {
                "1st",
                "2nd"
            };
            paths.ToList().ForEach(
                path => streamMock.Setup(x => x.Open(path))
                                               .Returns(new MemoryStream(new byte[] { 1, 2, 3 })));

            Mock<IWorkRecordReader> readerMock = new();
            var res = new List<WorkRecord>
            {
                TestWorkRecordFactory.Create(
                    workingDate: new DateTime(2023,4,1),
                    workingNumber: WorkingNumber.Create("23A-1")),
                TestWorkRecordFactory.Create(
                    workingDate: new DateTime(2023,4,1),
                    workingNumber: WorkingNumber.Create("23A-2")),
            };
            readerMock.Setup(x => x.ReadWorkRecordsAsync(It.IsAny<Stream>()))
                .ReturnsAsync(res);


            // when
            IReadAchieveTrackUseCase useCase = new ReadAchieveTrackUseCase(streamMock.Object,
                                                                           readerMock.Object);
            var actual = await useCase.ExecuteAsync(paths);

            // then
            var expected = paths.Select(_ => res.Select(x => WorkRecordAttempt.Parse(x)))
                                .SelectMany(x => x);
            CollectionAssert.AreEquivalent(expected.ToList(), actual.ToList());
            streamMock.Verify(x => x.Open(It.IsAny<string>()), Times.Exactly(2));
            readerMock.Verify(x => x.ReadWorkRecordsAsync(It.IsAny<Stream>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task 異常系_ストリームサイズが0のとき例外を返すこと()
        {
            // given
            Mock<IFileStreamOpener> streamMock = new();
            streamMock.Setup(x => x.Open(It.IsAny<string>()))
                .Returns(new MemoryStream());

            Mock<IWorkRecordReader> readerMock = new();

            // when
            IReadAchieveTrackUseCase useCase = new ReadAchieveTrackUseCase(streamMock.Object,
                                                                           readerMock.Object);
            var paths = new string[] { "1st" };
            Task target() => useCase.ExecuteAsync(paths!);

            // then
            var ex = await Assert.ThrowsExceptionAsync<ReadAchieveTrackUseCaseException>(target);
            var message = $"ファイルサイズが不正です パス: {paths[0]}";
            Assert.AreEqual(message, ex.Message);
        }
    }
}