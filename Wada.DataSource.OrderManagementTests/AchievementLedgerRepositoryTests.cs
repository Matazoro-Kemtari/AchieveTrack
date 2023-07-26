using Wada.DataSource.OrderManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;

namespace Wada.DataSource.OrderManagement.Tests
{
    [TestClass()]
    public class AchievementLedgerRepositoryTests
    {
        [TestMethod()]
        public void 正常系_実績台帳に追加できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IAchievementLedgerRepository> achievementMock = new();
            var achievementLedger = TestAchievementLedgerFacroty.Create();
            achievementMock.Setup(x => x.Add(It.IsAny<Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger>()))
                .Returns(achievementLedger.AchievementDetails.Count() + 1);

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(achievementMock.Object);
            var count = repository.Add(achievementLedger);

            // then
            Assert.AreEqual(achievementLedger.AchievementDetails.Count() + 1, count);
            achievementMock.Verify(x => x.Add(It.IsAny<Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger>()), Times.Once);
        }

        [TestMethod()]
        public void 異常系_実績台帳に追加できなかったとき例外を返すこと()
        {
            // given
            Mock<Data.OrderManagement.Models.IAchievementLedgerRepository> achievementMock = new();
            var achievementLedger = TestAchievementLedgerFacroty.Create();
            var message = $"実績台帳に登録できませんでした レコード: {achievementLedger}";
            achievementMock.Setup(x => x.Add(It.IsAny<Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger>()))
                .Throws(new Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedgerAggregationException(message));

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(achievementMock.Object);
            void target() => repository.Add(achievementLedger);

            // then
            var ex = Assert.ThrowsException<AchievementLedgerAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public async Task 正常系_作業日と社員番号で実績が取得できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IAchievementLedgerRepository> achievementMock = new();
            var achievementLedger = Data.OrderManagement.Models.AchievementLedgerAggregation.TestAchievementLedgerFacroty.Create();
            achievementMock.Setup(x => x.FindByWorkingDateAndEmployeeNumberAsync(achievementLedger.WorkingDate, achievementLedger.EmployeeNumber))
                .ReturnsAsync(achievementLedger);

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(achievementMock.Object);
            var actual = await repository.FindByWorkingDateAndEmployeeNumberAsync(achievementLedger.WorkingDate, achievementLedger.EmployeeNumber);

            // then
            var expected = AchievementLedger.Parse(achievementLedger);
            Assert.AreEqual(expected.AchievementId, actual.AchievementId);
            Assert.AreEqual(expected.WorkingDate, actual.WorkingDate);
            Assert.AreEqual(expected.EmployeeNumber, actual.EmployeeNumber);
            Assert.AreEqual(expected.DepartmentID, actual.DepartmentID);
            // プロパティ内の明細コレクションを抜いて比較
            CollectionAssert.AreEqual(
                expected.AchievementDetails.Select(x => new
                {
                    x.AchievementId,
                    x.OwnCompanyNumber,
                    x.AchievementProcessId,
                    x.ManHour,
                }).ToArray(),
                actual.AchievementDetails.Select(x => new
                {
                    x.AchievementId,
                    x.OwnCompanyNumber,
                    x.AchievementProcessId,
                    x.ManHour,
                }).ToArray());
        }

        [TestMethod()]
        public async Task 異常系_作業日と社員番号で実績台帳に該当がなかったとき例外を返すこと()
        {
            // given
            Mock<Data.OrderManagement.Models.IAchievementLedgerRepository> achievementMock = new();
            var achievementLedger = TestAchievementLedgerFacroty.Create();
            var message = $"作業日と社員番号を確認してください 実績台帳に登録されていません " +
                $"作業日: {achievementLedger.WorkingDate}, 社員番号: {achievementLedger.EmployeeNumber}";
            achievementMock.Setup(x => x.FindByWorkingDateAndEmployeeNumberAsync(achievementLedger.WorkingDate, achievementLedger.EmployeeNumber))
                .ThrowsAsync(new Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedgerAggregationException(message));

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(achievementMock.Object);
            Task target() => repository.FindByWorkingDateAndEmployeeNumberAsync(achievementLedger.WorkingDate, achievementLedger.EmployeeNumber);

            // then
            var ex = await Assert.ThrowsExceptionAsync<AchievementLedgerAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public async Task 正常系_実績IDが最大値の実績が取得できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IAchievementLedgerRepository> achievementMock = new();
            var achievementLedger = Data.OrderManagement.Models.AchievementLedgerAggregation.TestAchievementLedgerFacroty.Create();
            achievementMock.Setup(x => x.MaxByAchievementIdAsync())
                .ReturnsAsync(achievementLedger);

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(achievementMock.Object);
            var actual = await repository.MaxByAchievementIdAsync();

            // then
            var expected = AchievementLedger.Parse(achievementLedger);
            Assert.AreEqual(expected.AchievementId, actual.AchievementId);
            Assert.AreEqual(expected.WorkingDate, actual.WorkingDate);
            Assert.AreEqual(expected.EmployeeNumber, actual.EmployeeNumber);
            Assert.AreEqual(expected.DepartmentID, actual.DepartmentID);
            // プロパティ内の明細コレクションを抜いて比較
            CollectionAssert.AreEqual(
                expected.AchievementDetails.Select(x => new
                {
                    x.AchievementId,
                    x.OwnCompanyNumber,
                    x.AchievementProcessId,
                    x.ManHour,
                }).ToArray(),
                actual.AchievementDetails.Select(x => new
                {
                    x.AchievementId,
                    x.OwnCompanyNumber,
                    x.AchievementProcessId,
                    x.ManHour,
                }).ToArray());
        }

        [TestMethod()]
        public async Task 異常系_実績台帳レコードが無いときに最大値を取得したとき例外を返すこと()
        {
            // given
            Mock<Data.OrderManagement.Models.IAchievementLedgerRepository> achievementMock = new();
            var message = "実績台帳が登録されていません";
            achievementMock.Setup(x => x.MaxByAchievementIdAsync())
                .ThrowsAsync(new Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedgerAggregationException(message));

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(achievementMock.Object);
            Task target() => repository.MaxByAchievementIdAsync();

            // then
            var ex = await Assert.ThrowsExceptionAsync<AchievementLedgerAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }
    }
}