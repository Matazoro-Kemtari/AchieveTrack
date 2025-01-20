using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Wada.AchieveTrackService.DesignManagementAggregation;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkOrderAggregation;
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

            var workOrder = TestWorkOrderFactory.Create();

            Mock<IWorkOrderRepository> workOrderMock = new();
            workOrderMock.Setup(x => x.FindByWorkOrderIdAsync(It.IsAny<WorkOrderId>()))
                .ReturnsAsync(workOrder);

            Mock<IAchievementLedgerRepository> achievementMock = new();
            achievementMock.Setup(x => x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(), It.IsAny<uint>()))
                .ThrowsAsync(new AchievementLedgerAggregationException());

            Mock<IDesignManagementRepository> designMock = new();

            // when
            WorkRecordValidator validator = new(workOrderMock.Object,
                                                achievementMock.Object,
                                                designMock.Object);
            var actual = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            Assert.IsFalse(actual.SelectMany(x => x).Any());
            workOrderMock.Verify(
                x => x.FindByWorkOrderIdAsync(It.IsAny<WorkOrderId>()), Times.Exactly(3 * workRecords.Count));
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

            Mock<IWorkOrderRepository> workOrderMock = new();
            workOrderMock.Setup(x => x.FindByWorkOrderIdAsync(It.IsAny<WorkOrderId>()))
                .ThrowsAsync(new WorkOrderNotFoundException());

            Mock<IAchievementLedgerRepository> achievementMock = new();

            Mock<IDesignManagementRepository> designMock = new();

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workOrderMock.Object,
                                                                     achievementMock.Object,
                                                                     designMock.Object);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            var expected = workRecords.First();
            var actual = results.SelectMany(x => x);
            Assert.IsTrue(actual.Any(x => typeof(InvalidWorkOrderIdError) == x.GetType()));
            Assert.IsTrue(actual.Any(x => typeof(DuplicateWorkDateEmployeeError) == x.GetType()));
            actual.ToList().ForEach(x =>
            {
                Assert.AreEqual(expected.WorkOrderId.Value, x.WorkOrderId.Value);
                Assert.AreEqual(expected.Note, x.Note);
            });

            workOrderMock.Verify(
                x => x.FindByWorkOrderIdAsync(It.IsAny<WorkOrderId>()), Times.Once);
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
            var workOrder = TestWorkOrderFactory.Create(completionDate: workingDate.AddDays(-1));

            Mock<IWorkOrderRepository> workOrderMock = new();
            workOrderMock.Setup(x => x.FindByWorkOrderIdAsync(It.IsAny<WorkOrderId>()))
                .ReturnsAsync(workOrder);

            Mock<IAchievementLedgerRepository> achievementMock = new();

            Mock<IDesignManagementRepository> designMock = new();
            designMock.Setup(x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()))
                .ThrowsAsync(new DesignManagementNotFoundException());

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workOrderMock.Object,
                                                                     achievementMock.Object,
                                                                     designMock.Object);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            var expected = workRecords.First();
            var actual = results.SelectMany(x => x);
            Assert.IsTrue(actual.Any(x => typeof(WorkDateExpiredError) == x.GetType()));
            Assert.IsTrue(actual.Any(x => typeof(UnregisteredWorkOrderIdError) == x.GetType()));
            Assert.IsTrue(actual.Any(x => typeof(DuplicateWorkDateEmployeeError) == x.GetType()));
            actual.ToList().ForEach(x =>
            {
                Assert.AreEqual(expected.WorkOrderId.Value, x.WorkOrderId.Value);
                Assert.AreEqual(expected.Note, x.Note);
            });

            workOrderMock.Verify(
                x => x.FindByWorkOrderIdAsync(It.IsAny<WorkOrderId>()), Times.Exactly(3 * workRecords.Count));
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
            var workOrder = TestWorkOrderFactory.Create(completionDate: workingDate.AddDays(-1));

            Mock<IWorkOrderRepository> workOrderMock = new();
            workOrderMock.Setup(x => x.FindByWorkOrderIdAsync(It.IsAny<WorkOrderId>()))
                .ReturnsAsync(workOrder);

            Mock<IAchievementLedgerRepository> achievementMock = new();

            Mock<IDesignManagementRepository> designMock = new();

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workOrderMock.Object,
                                                                     achievementMock.Object,
                                                                     designMock.Object);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            var expected = workRecords.First();
            var actual = results.SelectMany(x => x);
            Assert.IsTrue(actual.Any(x => typeof(WorkDateExpiredError) == x.GetType()));
            actual.ToList().ForEach(x =>
            {
                Assert.AreEqual(expected.WorkOrderId.Value, x.WorkOrderId.Value);
                Assert.AreEqual(expected.Note, x.Note);
            });

            workOrderMock.Verify(
                x => x.FindByWorkOrderIdAsync(It.IsAny<WorkOrderId>()), Times.Exactly(3));
            achievementMock.Verify(
                x => x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(),
                                                               It.IsAny<uint>()), Times.Once);
            designMock.Verify(
                x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()), Times.Once);
        }
    }
}