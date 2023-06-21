using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.WorkRecordReader;
using Wada.AchieveTrackService.ValueObjects;
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

            var paths = new string[]
            {
                "1st",
                "2nd"
            };

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
    }
}