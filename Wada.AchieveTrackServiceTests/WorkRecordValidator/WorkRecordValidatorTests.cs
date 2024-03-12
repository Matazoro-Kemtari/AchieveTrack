using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.DesignManagementAggregation;
using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;
using Wada.AchieveTrackService.WorkRecordReader;

namespace Wada.AchieveTrackService.WorkRecordValidator.Tests
{
    [TestClass()]
    public class WorkRecordValidatorTests
    {
        [TestMethod()]
        public async Task 正常系_作業日報の検証が通ること()
        {
            // given
            List<WorkRecord> workRecords = new()
            {
                TestWorkRecordFactory.Create(processFlow: "CAD"),
                TestWorkRecordFactory.Create(processFlow: "CAD"),
                TestWorkRecordFactory.Create(processFlow: "CAD"),
            };

            var workingLedger = TestWorkingLedgerFactory.Create();

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            Mock<IAchievementLedgerRepository> achievementMock = new();
            achievementMock.Setup(x => x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(), It.IsAny<uint>()))
                .ThrowsAsync(new AchievementLedgerAggregationException());

            Mock<IDesignManagementRepository> designMock = new();

            var employee = TestEmployeeFactory.Create(processFlowId: 2u);
            Mock<IEmployeeRepository> employeeMock = new();
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ReturnsAsync(employee);

            // when
            WorkRecordValidator validator = new(workingLedgerMock.Object,
                                                achievementMock.Object,
                                                designMock.Object,
                                                employeeMock.Object);
            var actual = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            Assert.IsFalse(actual.SelectMany(x => x).Any());
            workingLedgerMock.Verify(
                x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()), Times.Exactly(3 * workRecords.Count));
            achievementMock.Verify(
                x => x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(),
                                                               It.IsAny<uint>()), Times.Exactly(workRecords.Count));
            designMock.Verify(
                x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()), Times.Exactly(workRecords.Count));
        }

        [TestMethod()]
        public async Task 正常系_作業台帳にない_実績台帳に登録済みの異常があることを検出すること()
        {
            // given
            List<WorkRecord> workRecords = new()
            {
                TestWorkRecordFactory.Create(),
            };

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ThrowsAsync(new WorkingLedgerNotFoundException());

            Mock<IAchievementLedgerRepository> achievementMock = new();

            Mock<IDesignManagementRepository> designMock = new();

            Mock<IEmployeeRepository> employeeMock = new();

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workingLedgerMock.Object,
                                                                     achievementMock.Object,
                                                                     designMock.Object,
                                                                     employeeMock.Object);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            var expected = workRecords.First();
            var actual = results.SelectMany(x => x);
            Assert.IsTrue(actual.Any(x => typeof(InvalidWorkNumberResult) == x.GetType()));
            Assert.IsTrue(actual.Any(x => typeof(DuplicateWorkDateEmployeeResult) == x.GetType()));
            actual.ToList().ForEach(x =>
            {
                Assert.AreEqual(expected.WorkingNumber.Value, x.WorkingNumber.Value);
                Assert.AreEqual(expected.Note, x.Note);
            });

            workingLedgerMock.Verify(
                x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()), Times.Once);
            achievementMock.Verify(
                x => x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(),
                                                               It.IsAny<uint>()), Times.Once);
            designMock.Verify(
                x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()), Times.Never);
        }

        [TestMethod()]
        public async Task 正常系_完成日過ぎている_設計管理に登録されていない_実績台帳に登録済みの異常があることを検出すること()
        {
            // given
            var workingDate = new DateTime(2023, 4, 1);
            List<WorkRecord> workRecords = new()
            {
                TestWorkRecordFactory.Create(workingDate: workingDate,
                                             processFlow: "CAD"),
            };
            var workingLedger = TestWorkingLedgerFactory.Create(completionDate: workingDate.AddDays(-1));

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            Mock<IAchievementLedgerRepository> achievementMock = new();

            Mock<IDesignManagementRepository> designMock = new();
            designMock.Setup(x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()))
                .ThrowsAsync(new DesignManagementNotFoundException());

            var employee = TestEmployeeFactory.Create(processFlowId: 2u);
            Mock<IEmployeeRepository> employeeMock = new();
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ReturnsAsync(employee);

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workingLedgerMock.Object,
                                                                     achievementMock.Object,
                                                                     designMock.Object,
                                                                     employeeMock.Object);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            var expected = workRecords.First();
            var actual = results.SelectMany(x => x);
            Assert.IsTrue(actual.Any(x => typeof(WorkDateExpiredResult) == x.GetType()));
            Assert.IsTrue(actual.Any(x => typeof(UnregisteredWorkNumberResult) == x.GetType()));
            Assert.IsTrue(actual.Any(x => typeof(DuplicateWorkDateEmployeeResult) == x.GetType()));
            actual.ToList().ForEach(x =>
            {
                Assert.AreEqual(expected.WorkingNumber.Value, x.WorkingNumber.Value);
                Assert.AreEqual(expected.Note, x.Note);
            });

            workingLedgerMock.Verify(
                x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()), Times.Exactly(3 * workRecords.Count));
            achievementMock.Verify(
                x => x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(),
                                                               It.IsAny<uint>()), Times.Once);
            designMock.Verify(
                x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()), Times.Exactly(workRecords.Count));
        }

        [TestMethod()]
        public async Task 正常系_作業日が完成日を過ぎていることを検出すること()
        {
            // given
            var workingDate = new DateTime(2023, 4, 1);
            List<WorkRecord> workRecords = new()
            {
                TestWorkRecordFactory.Create(workingDate: workingDate,
                                             processFlow: "CAD"),
            };
            var workingLedger = TestWorkingLedgerFactory.Create(completionDate: workingDate.AddDays(-1));

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            Mock<IAchievementLedgerRepository> achievementMock = new();

            Mock<IDesignManagementRepository> designMock = new();

            var employee = TestEmployeeFactory.Create(processFlowId: 2u);
            Mock<IEmployeeRepository> employeeMock = new();
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ReturnsAsync(employee);

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workingLedgerMock.Object,
                                                                     achievementMock.Object,
                                                                     designMock.Object,
                                                                     employeeMock.Object);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            var expected = workRecords.First();
            var actual = results.SelectMany(x => x);
            Assert.IsTrue(actual.Any(x => typeof(WorkDateExpiredResult) == x.GetType()));
            actual.ToList().ForEach(x =>
            {
                Assert.AreEqual(expected.WorkingNumber.Value, x.WorkingNumber.Value);
                Assert.AreEqual(expected.Note, x.Note);
            });

            workingLedgerMock.Verify(
                x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()), Times.Exactly(3));
            achievementMock.Verify(
                x => x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(),
                                                               It.IsAny<uint>()), Times.Once);
            designMock.Verify(
                x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()), Times.Once);
        }
    }
}