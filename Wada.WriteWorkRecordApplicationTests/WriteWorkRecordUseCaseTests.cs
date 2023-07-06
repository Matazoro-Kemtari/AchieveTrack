using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;

namespace Wada.WriteWorkRecordApplication.Tests
{
    [TestClass()]
    public class WriteWorkRecordUseCaseTests
    {
        [TestMethod()]
        public async Task 正常系_例外なく更新処理が終わること()
        {
            // given
            Mock<IEmployeeReader> employeeMock = new();
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ReturnsAsync(TestEmployeeFactory.Create());

            Mock<IWorkingLedgerReader> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(TestWorkingLedgerFactory.Create());

            Mock<IAchievementLedgerRepository> achievementMock = new();
            achievementMock.Setup(x => x.AddAsync(It.IsAny<AchievementLedger>()))
                .ReturnsAsync(1);

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock.Object,
                workingLedgerMock.Object,
                achievementMock.Object);
            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(),
                TestAchievementParamFactory.Create(),
            };
            var count = await useCase.ExecuteAsync(achievements);

            // then
            Assert.AreEqual(achievements.Count, count);
            employeeMock.Verify(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()), Times.Exactly(achievements.Count));
            workingLedgerMock.Verify(
                x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()), Times.Exactly(achievements.Sum(x => x.AchievementDetails.Count())));
            achievementMock.Verify(x => x.AddAsync(It.IsAny<AchievementLedger>()), Times.Exactly(achievements.Count));
        }

        [TestMethod()]
        public async Task 異常系_社員が見つからないとき例外を返すこと()
        {
            // given
            Mock<IEmployeeReader> employeeMock = new();
            string employeeMessage = "社員番号が見つかりません";
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ThrowsAsync(new EmployeeAggregationException(employeeMessage));

            Mock<IWorkingLedgerReader> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(TestWorkingLedgerFactory.Create());

            Mock<IAchievementLedgerRepository> achievementMock = new();

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock.Object,
                workingLedgerMock.Object,
                achievementMock.Object);
            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(),
                TestAchievementParamFactory.Create(),
            };
            Task target() => _ = useCase.ExecuteAsync(achievements!);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WriteWorkRecordUseCaseException>(target);
            var message = $"実績を登録中に問題が発生しました\n{employeeMessage}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public async Task 異常系_作業台帳が見つからないとき例外を返すこと()
        {
            // given
            Mock<IEmployeeReader> employeeMock = new();

            Mock<IWorkingLedgerReader> workingLedgerMock = new();
            string workingLedgerMessage = "作業台帳が見つかりません";
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ThrowsAsync(new WorkingLedgerAggregationException(workingLedgerMessage));

            Mock<IAchievementLedgerRepository> achievementMock = new();

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock.Object,
                workingLedgerMock.Object,
                achievementMock.Object);
            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(),
                TestAchievementParamFactory.Create(),
            };
            Task target() => _ = useCase.ExecuteAsync(achievements!);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WriteWorkRecordUseCaseException>(target);
            var message = $"実績を登録中に問題が発生しました\n{workingLedgerMessage}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public async Task 異常系_実績台帳に登録できなかったとき例外を返すこと()
        {
            // given
            Mock<IEmployeeReader> employeeMock = new();
            var testEmployee = TestEmployeeFactory.Create();
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ReturnsAsync(testEmployee);

            Mock<IWorkingLedgerReader> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(TestWorkingLedgerFactory.Create());

            Mock<IAchievementLedgerRepository> achievementMock = new();
            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(),
                TestAchievementParamFactory.Create(),
            };
            string achievementMessage =
                $"実績日: {achievements.First().WorkingDate:yyyy/MM/dd(ddd)}\n" +
                $"氏名: {testEmployee.Name}\n" +
                $"作業番号: {achievements.First().AchievementDetails.First().WorkingNumber}";
            achievementMock.Setup(x => x.AddAsync(It.IsAny<AchievementLedger>()))
                .ThrowsAsync(new AchievementLedgerAggregationException(achievementMessage));

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock.Object,
                workingLedgerMock.Object,
                achievementMock.Object);
            Task target() => _ = useCase.ExecuteAsync(achievements!);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WriteWorkRecordUseCaseException>(target);
            var message = $"実績を登録中に問題が発生しました\n{achievementMessage}";
            Assert.AreEqual(message, ex.Message);
        }
    }

}