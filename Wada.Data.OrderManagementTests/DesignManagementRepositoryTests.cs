using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.DesignManagementAggregation;

namespace Wada.Data.OrderManagement.Tests
{
    [TestClass()]
    public class DesignManagementRepositoryTests
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
        public void 正常系_設計管理に追加できること()
        {
            // given
            using TransactionScope scope = new();

            var record = TestDesignManagementFactory.Create(ownCompanyNumber: int.MaxValue);

            // when
            IDesignManagementRepository repository = new DesignManagementRepository(_configuration!);
            var actual = repository.Add(record);
            var additionalItem = repository.FindByOwnCompanyNumber(int.MaxValue);

            // then
            Assert.AreEqual(1, actual);
            Assert.AreEqual(record, additionalItem);
        }

        [TestMethod()]
        public void 異常系_設計管理に追加できなかったとき例外を返すこと()
        {
            // given
            using TransactionScope scope = new();

            IDesignManagementRepository repository = new DesignManagementRepository(_configuration!);
            var existItem = repository.FindAll().Last();
            var additionalItem = TestDesignManagementFactory.Create(ownCompanyNumber: existItem.OwnCompanyNumber);

            // when
            void target() => repository.Add(additionalItem);

            // then
            var ex = Assert.ThrowsException<DesignManagementAggregationException>(target);
            var message = "設計管理に登録できませんでした 自社NOを確認してください " +
                $"自社NO: {additionalItem.OwnCompanyNumber}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod()]
        public void 異常系_自社NOに該当がなかったとき例外を返すこと()
        {
            // given
            IDesignManagementRepository repository = new DesignManagementRepository(_configuration!);

            var all = repository.FindAll();
            var maxId = all.Max(x => x.OwnCompanyNumber) + 1u;

            // when
            void target() => _ = repository.FindByOwnCompanyNumber(maxId);

            // then
            var ex = Assert.ThrowsException<DesignManagementNotFoundException>(target);
            var message = $"設計管理に該当がありません 自社NOを確認してください 自社NO: {maxId}";
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod]
        public async Task 正常系_非同期_自社NOで設計管理が取得できること()
        {
            // given
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            var record = TestDesignManagementFactory.Create(ownCompanyNumber: int.MaxValue);

            // when
            IDesignManagementRepository repository = new DesignManagementRepository(_configuration!);
            var actual = repository.Add(record);
            var additionalItem = await repository.FindByOwnCompanyNumberAsync(int.MaxValue);

            // then
            Assert.AreEqual(1, actual);
            Assert.AreEqual(record, additionalItem);
        }

        [TestMethod()]
        public async Task 異常系_非同期_自社NOに該当がなかったとき例外を返すこと()
        {
            // given
            IDesignManagementRepository repository = new DesignManagementRepository(_configuration!);

            var all = repository.FindAll();
            var maxId = all.Max(x => x.OwnCompanyNumber) + 1u;

            // when
            Task target() => _ = repository.FindByOwnCompanyNumberAsync(maxId);

            // then
            var ex = await Assert.ThrowsExceptionAsync<DesignManagementNotFoundException>(target);
            var message = $"設計管理に該当がありません 自社NOを確認してください 自社NO: {maxId}";
            Assert.AreEqual(message, ex.Message);
        }
    }
}