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
        public async Task 正常系_実績台帳に追加できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IAchievementLedgerRepository> achievementMock = new();
            var achievementLedger = TestAchievementLedgerFacroty.Create();
            achievementMock.Setup(x => x.AddAsync(It.IsAny<Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger>()))
                .ReturnsAsync(achievementLedger.AchievementDetails.Count() + 1);

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(achievementMock.Object);
            var count = await repository.AddAsync(achievementLedger);

            // then
            Assert.AreEqual(achievementLedger.AchievementDetails.Count() + 1, count);
            achievementMock.Verify(x => x.AddAsync(It.IsAny<Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger>()), Times.Once);
        }

        [TestMethod()]
        public async Task 正常系_実績台帳を全件取得できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IAchievementLedgerRepository> achievementMock = new();
            var achievementLedgers = new List<Data.OrderManagement.Models.AchievementLedgerAggregation.AchievementLedger>
            {
                Data.OrderManagement.Models.AchievementLedgerAggregation.TestAchievementLedgerFacroty.Create(),
            };
            achievementMock.Setup(x => x.FindAllAsync())
                .ReturnsAsync(achievementLedgers);

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(achievementMock.Object);
            var results = await repository.FindAllAsync();

            // then
            var convertedAchievementLedger = achievementLedgers.Select(x => AchievementLedger.Parse(x));
            var expected = convertedAchievementLedger.Select(x => new
            {
                x.Id,
                x.AchievementId,
                x.WorkingDate,
                x.EmployeeNumber,
                x.DepartmentID,
            });
            var actual = results.Select(x => new
            {
                x.Id,
                x.AchievementId,
                x.WorkingDate,
                x.EmployeeNumber,
                x.DepartmentID,
            });
            // プロパティ内の明細コレクションを抜いて比較
            CollectionAssert.AreEqual(expected.ToArray(),
                                      actual.ToArray());
            // 明細コレクションを比較
            CollectionAssert.AreEqual(convertedAchievementLedger.SelectMany(x => x.AchievementDetails).ToArray(),
                                      results.SelectMany(x => x.AchievementDetails).ToList());
            achievementMock.Verify(x => x.FindAllAsync(), Times.Once);
        }

        // TODO: 異常のテスト
    }
}