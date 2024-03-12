using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.DataBase.EFCore.OrderManagement.Entities;

namespace Wada.DataBase.EFCore.OrderManagement.Tests
{
    [TestClass]
    public class PartDetailLedgerForPurchaseItemTests
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
        public void 正常系_部品明細台帳購入品のデーダが取得できること()
        {
            // given
            // when
            List<PartDetailLedgerForPurchaseItem> actual;
            using (var dbContext = new OrderManagementContext(_configuration!))
            {
                actual = dbContext.PartDetailLedgerForPurchaseItems.ToList();
            }

            // then
            Assert.IsTrue(actual.Any());
        }
    }
}