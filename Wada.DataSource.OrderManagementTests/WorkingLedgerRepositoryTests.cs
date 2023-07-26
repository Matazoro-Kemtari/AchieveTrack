using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;

namespace Wada.DataSource.OrderManagement.Tests
{
    [TestClass()]
    public class WorkingLedgerRepositoryTests
    {
        [TestMethod()]
        public async Task 正常系_作業台帳が取得できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IWorkingLedgerRepository> workingMock = new();
            var workingLedger = Data.OrderManagement.Models.WorkingLedgerAggregation.TestWorkingLedgerFactory.Create();
            workingMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<Data.OrderManagement.Models.ValueObjects.WorkingNumber>()))
                .ReturnsAsync(workingLedger);

            // when
            IWorkingLedgerRepository repository = new WorkingLedgerRepository(workingMock.Object);
            var actual = await repository.FindByWorkingNumberAsync(WorkingNumber.Parse(workingLedger.WorkingNumber));

            // then
            Assert.IsNotNull(actual);
            var expected = WorkingLedger.Parse(workingLedger);
            Assert.AreEqual(expected, actual);
            workingMock.Verify(x => x.FindByWorkingNumberAsync(It.IsAny<Data.OrderManagement.Models.ValueObjects.WorkingNumber>()), Times.Once);
        }

        [TestMethod()]
        public async Task 異常系_作業台帳に作業番号がない場合例外を返すこと()
        {
            // given
            Mock<Data.OrderManagement.Models.IWorkingLedgerRepository> workingMock = new();
            var message = "作業番号を確認してください 受注管理に登録されていません";
            workingMock.Setup(x => x.FindByWorkingNumberAsync(It.IsAny<Data.OrderManagement.Models.ValueObjects.WorkingNumber>()))
                .ThrowsAsync(new Data.OrderManagement.Models.WorkingLedgerAggregation.WorkingLedgerAggregationException(message));

            // when
            IWorkingLedgerRepository repository = new WorkingLedgerRepository(workingMock.Object);
            var workingLedger = TestWorkingLedgerFactory.Create();
            Task target() => repository.FindByWorkingNumberAsync(workingLedger!.WorkingNumber);

            // then
            var ex = await Assert.ThrowsExceptionAsync<WorkingLedgerAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }
    }
}