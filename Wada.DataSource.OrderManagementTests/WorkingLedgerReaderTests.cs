using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService.WorkingLedgerAggregation;
using Wada.Data.OrderManagement.Models;
using Wada.Data.OrderManagement.Models.ValueObjects;
using Wada.WriteWorkRecordApplication;

namespace Wada.DataSource.OrderManagement.Tests
{
    [TestClass()]
    public class WorkingLedgerReaderTests
    {
        [TestMethod()]
        public async Task 正常系_作業台帳が取得できること()
        {
            // given
            Mock<IWorkingLedgerRepository> workingMock = new();
            var workingLedger = Data.OrderManagement.Models.WorkingLedgerAggregation.TestWorkingLedgerFactory.Create();
            workingMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            // when
            IWorkingLedgerReader reader = new WorkingLedgerReader(workingMock.Object);
            var actual = await reader.FindByWorkingNumberAsync(AchieveTrackService.ValueObjects.WorkingNumber.Parse(workingLedger.WorkingNumber));

            // then
            Assert.IsNotNull(actual);
            var expected = WorkingLedger.Parse(workingLedger);
            Assert.AreEqual(expected, actual);
            workingMock.Verify(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()), Times.Once);
        }

        [TestMethod()]
        public async Task 異常系_作業台帳に作業番号がない場合例外を返すこと()
        {
            // given
            Mock<IWorkingLedgerRepository> workingMock = new();
            var message = "作業番号を確認してください 受注管理に登録されていません";
            workingMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<WorkingNumber>()))
                .ThrowsAsync(new Data.OrderManagement.Models.WorkingLedgerAggregation.WorkingLedgerAggregationException(message));

            // when
            IWorkingLedgerReader reader = new WorkingLedgerReader(workingMock.Object);
            var workingLedger = TestWorkingLedgerFactory.Create();
            Task target() => reader.FindByWorkingNumberAsync(workingLedger!.WorkingNumber);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WorkingLedgerAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }
    }
}