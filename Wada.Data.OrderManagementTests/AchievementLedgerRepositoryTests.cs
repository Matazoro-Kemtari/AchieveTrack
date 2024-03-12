using Wada.Data.OrderManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.AchievementLedgerAggregation;
using Microsoft.Extensions.Configuration;
using System.Transactions;

namespace Wada.Data.OrderManagement.Tests
{
    [TestClass()]
    public class AchievementLedgerRepositoryTests
    {
        private static IConfiguration? _configuration;

        [ClassInitialize]
        public static void ClassInitialize(TestContext _)
        {
            // NOTE: https://qiita.com/mima_ita/items/55394bcc851eb8b6dc24

            DotNetEnv.Env.Load(".env");
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }

        [TestMethod()]
        public void 正常系_実績台帳に追加できること()
        {
            // given
            using TransactionScope scope = new();

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(_configuration!);

            var before = repository.FindAll();

            var newId = before.Max(x => x.Id) + 1;
            var record = TestAchievementLedgerFacroty.Create(id: newId);

            var count = repository.Add(record);
            var response = repository.FindAll();

            // then
            var actual = response.Single(x => x.Id == newId);
            Assert.AreEqual(record.AchievementDetails.Count() + 1, count);
            Assert.IsNotNull(actual);
            Assert.AreEqual(newId, actual.Id);
            Assert.AreEqual(record.WorkingDate, actual.WorkingDate);
            Assert.AreEqual(record.EmployeeNumber, actual.EmployeeNumber);
            Assert.AreEqual(record.DepartmentID, actual.DepartmentID);
            CollectionAssert.AreEquivalent(
                record.AchievementDetails.ToList(),
                actual.AchievementDetails.ToList());
        }

        [TestMethod()]
        public void 異常系_実績台帳に追加できなかったとき例外を返すこと()
        {
            // given
            using TransactionScope scope = new();

            IAchievementLedgerRepository repository = new AchievementLedgerRepository(_configuration!);

            var before = repository.FindAll();

            var newId = before.Max(x => x.Id);

            // when
            var achievementLedger = TestAchievementLedgerFacroty.Create(id: newId);
            void target() => repository.Add(achievementLedger);

            // then
            var ex = Assert.ThrowsException<AchievementLedgerAggregationException>(target);
            var message = $"実績台帳に登録できませんでした レコード: {achievementLedger}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public async Task 正常系_作業日と社員番号で実績が取得できること()
        {
            // given
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            IAchievementLedgerRepository repository = new AchievementLedgerRepository(_configuration!);

            var workingDate = DateTime.Now.Date;
            var employeeNumber = 4001u;
            var expected = TestAchievementLedgerFacroty.Create(workingDate: workingDate, employeeNumber: employeeNumber);

            _ = repository.Add(expected);

            // when
            var actual = await repository.FindByWorkingDateAndEmployeeNumberAsync(workingDate, employeeNumber);


            // then
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.WorkingDate, actual.WorkingDate);
            Assert.AreEqual(expected.EmployeeNumber, actual.EmployeeNumber);
            Assert.AreEqual(expected.DepartmentID, actual.DepartmentID);
            // プロパティ内の明細コレクションを比較
            CollectionAssert.AreEqual(
                expected.AchievementDetails.ToArray(),
                actual.AchievementDetails.ToArray());
        }

        [TestMethod()]
        public async Task 異常系_作業日と社員番号で実績台帳に該当がなかったとき例外を返すこと()
        {
            // given
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            var achievementLedger = TestAchievementLedgerFacroty.Create();

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(_configuration!);
            Task target() => repository.FindByWorkingDateAndEmployeeNumberAsync(achievementLedger.WorkingDate, achievementLedger.EmployeeNumber);

            // then
            var ex = await Assert.ThrowsExceptionAsync<AchievementLedgerAggregationException>(target);
            var message = $"作業日と社員番号を確認してください 実績台帳に登録されていません " +
                $"作業日: {achievementLedger.WorkingDate}, 社員番号: {achievementLedger.EmployeeNumber}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public async Task 正常系_実績IDが最大値の実績が取得できること()
        {
            // given
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            // when
            IAchievementLedgerRepository repository = new AchievementLedgerRepository(_configuration!);
            var actual = await repository.MaxByAchievementIdAsync();

            // then
            var achievementLedgers = repository.FindAll();
            var expected = achievementLedgers.MaxBy(x => x.Id);
            if (expected is null)
                Assert.Fail();
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.WorkingDate, actual.WorkingDate);
            Assert.AreEqual(expected.EmployeeNumber, actual.EmployeeNumber);
            Assert.AreEqual(expected.DepartmentID, actual.DepartmentID);
            // プロパティ内の明細コレクションを抜いて比較
            CollectionAssert.AreEqual(
                expected.AchievementDetails.ToArray(),
                actual.AchievementDetails.ToArray());
        }
    }
}