﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkRecordReader;
using Wada.Data.OrderManagement.Models;
using Wada.Data.OrderManagement.Models.AchievementLedgerAggregation;
using Wada.Data.OrderManagement.Models.DesignManagementAggregation;
using Wada.Data.OrderManagement.Models.WorkingLedgerAggregation;

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
                TestWorkRecordFactory.Create(),
                TestWorkRecordFactory.Create(),
                TestWorkRecordFactory.Create(),
            };

            var workingLedger = TestWorkingLedgerFactory.Create();

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            Mock<IAchievementLedgerRepository> achievementMock = new();
            achievementMock.Setup(x => x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(), It.IsAny<uint>()))
                .ThrowsAsync(new AchievementLedgerAggregationException());

            var designMock = Mock.Of<IDesignManagementRepository>();

            // when
            WorkRecordValidator validator = new(workingLedgerMock.Object, achievementMock.Object, designMock);
            var actual = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            Assert.IsTrue(actual.All(x => x.GetType() == typeof(ValidationSuccessResult)));
        }

        [TestMethod()]
        public async Task 正常系_作業番号が台帳にない場合検出すること()
        {
            // given
            List<WorkRecord> workRecords = new()
            {
                TestWorkRecordFactory.Create(),
            };

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ThrowsAsync(new WorkingLedgerAggregationException());

            var achievementMock = Mock.Of<IAchievementLedgerRepository>();
            var designMock = Mock.Of<IDesignManagementRepository>();

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workingLedgerMock.Object, achievementMock, designMock);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            results.ToList().ForEach(
                x => Assert.IsInstanceOfType<InvalidWorkNumberResult>(x));
        }

        [TestMethod()]
        public async Task 正常系_作業日が完成日を過ぎている場合検出すること()
        {
            // given
            var workingDate = new DateTime(2023, 4, 1);
            List<WorkRecord> workRecords = new()
            {
                TestWorkRecordFactory.Create(workingDate: workingDate),
            };
            var workingLedger = TestWorkingLedgerFactory.Create(completionDate: workingDate.AddDays(-1));

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            var achievementMock = Mock.Of<IAchievementLedgerRepository>();
            var designMock = Mock.Of<IDesignManagementRepository>();

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workingLedgerMock.Object, achievementMock, designMock);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            results.ToList().ForEach(
                x => Assert.IsInstanceOfType<WorkDateExpiredResult>(x));
        }

        [TestMethod()]
        public async Task 正常系_実績台帳に登録済みの場合検出すること()
        {
            // given
            List<WorkRecord> workRecords = new()
            {
                TestWorkRecordFactory.Create(),
            };

            var workingLedger = TestWorkingLedgerFactory.Create();

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            Mock<IAchievementLedgerRepository> achievementMock = new();

            var designMock = Mock.Of<IDesignManagementRepository>();

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workingLedgerMock.Object, achievementMock.Object, designMock);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            results.ToList().ForEach(
                x => Assert.IsInstanceOfType<DuplicateWorkDateEmployeeResult>(x));
        }

        [TestMethod()]
        public async Task 正常系_設計管理に未登録の場合検出すること()
        {
            // given
            List<WorkRecord> workRecords = new()
            {
                TestWorkRecordFactory.Create(),
            };

            var workingLedger = TestWorkingLedgerFactory.Create();

            Mock<IWorkingLedgerRepository> workingLedgerMock = new();
            workingLedgerMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            Mock<IAchievementLedgerRepository> achievementMock = new();
            achievementMock.Setup(x=>x.FindByWorkingDateAndEmployeeNumberAsync(It.IsAny<DateTime>(), It.IsAny<uint>()))
                .ThrowsAsync(new AchievementLedgerAggregationException());

            Mock<IDesignManagementRepository> designMock = new();
            designMock.Setup(x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()))
                .ThrowsAsync(new DesignManagementAggregationException());

            // when
            IWorkRecordValidator validator = new WorkRecordValidator(workingLedgerMock.Object, achievementMock.Object, designMock.Object);
            var results = await validator.ValidateWorkRecordsAsync(workRecords);

            // then
            results.ToList().ForEach(
                x => Assert.IsInstanceOfType<UnregisteredWorkNumberResult>(x));
        }
    }
}