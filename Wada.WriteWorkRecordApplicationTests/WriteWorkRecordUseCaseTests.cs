using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.AchieveTrackService.ProcessFlowAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;

namespace Wada.WriteWorkRecordApplication.Tests
{
    [TestClass()]
    public class WriteWorkRecordUseCaseTests
    {
        [DataTestMethod()]
        [DataRow(true, 1)]
        [DataRow(false, 1)]
        [DataRow(true, 2)]  // 実績工程 2 = CAD
        [DataRow(false, 2)]
        public async Task 正常系_例外なく更新処理が終わること(bool canAdd, int processFlowId)
        {
            // given
            Mock<IEmployeeRepository> employeeMock = new();
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ReturnsAsync(TestEmployeeFactory.Create());

            Mock<IProcessFlowRepository> processMock = new();
            processMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(TestProcessFlowFactory.Create(id: (uint)processFlowId));

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(TestWorkingNumberFactory.Create("23Z-1")))
                .ReturnsAsync(TestWorkingLedgerFactory.Create(ownCompanyNumber: 101u,
                                                              workingNumber: TestWorkingNumberFactory.Create("23Z-1")));
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(TestWorkingNumberFactory.Create("23Z-2")))
                .ReturnsAsync(TestWorkingLedgerFactory.Create(ownCompanyNumber: 102u,
                                                              workingNumber: TestWorkingNumberFactory.Create("23Z-2")));

            Mock<IAchievementLedgerRepository> achievementMock = new();
            achievementMock.Setup(x => x.MaxByAchievementIdAsync())
                .ReturnsAsync(TestAchievementLedgerFactory.Create());
            achievementMock.Setup(x => x.Add(It.IsAny<AchievementLedger>()))
                .Returns(1);

            Mock<IDesignManagementWriter> designMock = new();

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock.Object,
                processMock.Object,
                workingLedgerMock.Object,
                achievementMock.Object,
                designMock.Object);
            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(
                    workingDate: new DateTime(2023, 4, 1),
                    achievementDetails: new[]
                    {
                        TestAchievementDetailParamFactory.Create(workingNumber: "23Z-1")
                    }),
                TestAchievementParamFactory.Create(
                    workingDate: new DateTime(2023, 12, 1),
                    achievementDetails: new[]
                    {
                        TestAchievementDetailParamFactory.Create(workingNumber: "23Z-1")
                    }),
                TestAchievementParamFactory.Create(
                    workingDate: new DateTime(2023, 5, 1),
                    achievementDetails: new[]
                    {
                        TestAchievementDetailParamFactory.Create(workingNumber: "23Z-2")
                    }),
            };
            var count = await useCase.ExecuteAsync(achievements, canAdd);

            // then
            Assert.AreEqual(achievements.Count, count);
            achievementMock.Verify(x => x.MaxByAchievementIdAsync(), Times.Once);
            employeeMock.Verify(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()), Times.Exactly(achievements.Count));
            workingLedgerMock.Verify(
                x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()), Times.Exactly(achievements.Sum(x => x.AchievementDetails.Count())));
            achievementMock.Verify(x => x.Add(It.IsAny<AchievementLedger>()), Times.Exactly(achievements.Count));
            if (canAdd && processFlowId == 2)
            {
                designMock.Verify(x => x.Add(It.IsAny<uint>(), new DateTime(2023, 4, 1)), Times.Once);
                designMock.Verify(x => x.Add(It.IsAny<uint>(), new DateTime(2023, 5, 1)), Times.Once);
                designMock.Verify(x => x.Add(It.IsAny<uint>(), new DateTime(2023, 12, 1)), Times.Never);
            }
            else
                designMock.Verify(x => x.Add(It.IsAny<uint>(), It.IsAny<DateTime>()), Times.Exactly(0));
        }

        [TestMethod()]
        public async Task 異常系_社員が見つからないとき例外を返すこと()
        {
            // given
            Mock<IEmployeeRepository> employeeMock = new();
            string employeeMessage = "社員番号が見つかりません";
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ThrowsAsync(new EmployeeNotFoundException(employeeMessage));

            var processMock = Mock.Of<IProcessFlowRepository>();
            var workingLedgerMock = Mock.Of<IWorkingLedgerRepository>();
            var achievementMock = Mock.Of<IAchievementLedgerRepository>();
            var designMock = Mock.Of<IDesignManagementWriter>();

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock.Object,
                processMock,
                workingLedgerMock,
                achievementMock,
                designMock);
            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(),
                TestAchievementParamFactory.Create(),
            };
            Task target() => _ = useCase.ExecuteAsync(achievements!, false);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WriteWorkRecordUseCaseException>(target);
            var message = $"実績を登録中に問題が発生しました\n{employeeMessage}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod]
        public async Task 異常系_工程実績が見つからないとき例外を返すこと()
        {
            var employeeMock = Mock.Of<IEmployeeRepository>();

            Mock<IProcessFlowRepository> processMock = new();
            string repositoryMessage = "実績工程が見つかりません 実績工程: CAD";
            processMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ThrowsAsync(new ProcessFlowNotFoundException(repositoryMessage));

            var workingLedgerMock = Mock.Of<IWorkingLedgerRepository>();
            var achievementMock = Mock.Of<IAchievementLedgerRepository>();
            var designMock = Mock.Of<IDesignManagementWriter>();

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock,
                processMock.Object,
                workingLedgerMock,
                achievementMock,
                designMock);
            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(
                    achievementDetails:
                    [
                        TestAchievementDetailParamFactory.Create(processFlow: "CAD"),
                    ]),
                TestAchievementParamFactory.Create(
                    achievementDetails:
                    [
                        TestAchievementDetailParamFactory.Create(processFlow: "CAD"),
                    ]),
            };
            Task target() => _ = useCase.ExecuteAsync(achievements!, false);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WriteWorkRecordUseCaseException>(target);
            var message = $"実績を登録中に問題が発生しました\n{repositoryMessage}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public async Task 異常系_作業台帳が見つからないとき例外を返すこと()
        {
            // given
            var employeeMock = Mock.Of<IEmployeeRepository>();
            var processMock = Mock.Of<IProcessFlowRepository>();

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            string workingLedgerMessage = "作業台帳が見つかりません";
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ThrowsAsync(new WorkingLedgerNotFoundException(workingLedgerMessage));

            var achievementMock = Mock.Of<IAchievementLedgerRepository>();
            var designMock = Mock.Of<IDesignManagementWriter>();

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock,
                processMock,
                workingLedgerMock.Object,
                achievementMock,
                designMock);
            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(),
                TestAchievementParamFactory.Create(),
            };
            Task target() => _ = useCase.ExecuteAsync(achievements!, false);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WriteWorkRecordUseCaseException>(target);
            var message = $"実績を登録中に問題が発生しました\n{workingLedgerMessage}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public async Task 異常系_実績台帳に登録できなかったとき例外を返すこと()
        {
            // given
            Mock<IEmployeeRepository> employeeMock = new();
            var testEmployee = TestEmployeeFactory.Create();
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ReturnsAsync(testEmployee);

            var processMock = Mock.Of<IProcessFlowRepository>();

            var workingLedgerMock = Mock.Of<IWorkingLedgerRepository>();

            Mock<IAchievementLedgerRepository> achievementMock = new();
            achievementMock.Setup(x => x.MaxByAchievementIdAsync())
                .ReturnsAsync(TestAchievementLedgerFactory.Create());

            Mock<IDesignManagementWriter> designMock = new();

            var achievements = new List<AchievementParam>
            {
                TestAchievementParamFactory.Create(),
                TestAchievementParamFactory.Create(),
            };
            string achievementMessage =
                $"実績日: {achievements.First().WorkingDate:yyyy/MM/dd(ddd)}\n" +
                $"氏名: {testEmployee.Name}\n" +
                $"作業番号: {achievements.First().AchievementDetails.First().WorkingNumber}";
            achievementMock.Setup(x => x.Add(It.IsAny<AchievementLedger>()))
                .Throws(new AchievementLedgerAggregationException(achievementMessage));

            // when
            IWriteWorkRecordUseCase useCase = new WriteWorkRecordUseCase(
                employeeMock.Object,
                processMock,
                workingLedgerMock,
                achievementMock.Object,
                designMock.Object);
            Task target() => _ = useCase.ExecuteAsync(achievements!, false);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WriteWorkRecordUseCaseException>(target);
            var message = $"実績を登録中に問題が発生しました\n{achievementMessage}";
            Assert.AreEqual(message, ex.Message);
        }
    }

}