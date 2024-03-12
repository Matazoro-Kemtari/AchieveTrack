using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using Wada.DataBase.EFCore.DesignDepartment.Entities;

namespace Wada.DataBase.EFCore.DesignDepartment.Tests
{
    [TestClass()]
    public class OrderReportHistoriesTests
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

        [TestMethod]
        public void 正常系_受注報告履歴のデータが取得できること()
        {
            // given
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var baseDate = DateTime.Now.Date;
            var expected = new List<OrderReportHistory>
            {
                new OrderReportHistory
                {
                    Id = Ulid.NewUlid().ToString(),
                    CheckingDate = baseDate.AddDays(-1),
                    OrderReportHistoriesAmounts = new[]
                    {
                        new OrderReportHistoryAmount()
                        {
                            Id = Ulid.NewUlid().ToString(),
                            OrderYear = baseDate.AddDays(-1).Year,
                            OrderMonth = baseDate.AddDays(-1).Month,
                            ProspectiveSalesOrderAmount= 100m,
                            SalesOrderAmount= 1000m,
                        },
                        new OrderReportHistoryAmount()
                        {
                            Id = Ulid.NewUlid().ToString(),
                            OrderYear = baseDate.AddDays(-1).AddMonths(1).Year,
                            OrderMonth = baseDate.AddDays(-1).AddMonths(1).Month,
                            ProspectiveSalesOrderAmount= 100m,
                            SalesOrderAmount= 1000m,
                        },
                        new OrderReportHistoryAmount()
                        {
                            Id = Ulid.NewUlid().ToString(),
                            OrderYear = baseDate.AddDays(-1).AddMonths(2).Year,
                            OrderMonth = baseDate.AddDays(-1).AddMonths(2).Month,
                            ProspectiveSalesOrderAmount= 100m,
                            SalesOrderAmount= 1000m,
                        },
                    }
                },
                new OrderReportHistory
                {
                    Id = Ulid.NewUlid().ToString(),
                    CheckingDate =baseDate ,
                    OrderReportHistoriesAmounts=new[]
                    {
                        new OrderReportHistoryAmount()
                        {
                            Id = Ulid.NewUlid().ToString(),
                            OrderYear = baseDate.Year,
                            OrderMonth = baseDate.Month,
                            ProspectiveSalesOrderAmount = 99m,
                            SalesOrderAmount = 999m
                        },
                        new OrderReportHistoryAmount()
                        {
                            Id = Ulid.NewUlid().ToString(),
                            OrderYear = baseDate.AddMonths(1).Year,
                            OrderMonth = baseDate.AddMonths(1).Month,
                            ProspectiveSalesOrderAmount = 99m,
                            SalesOrderAmount = 999m
                        },
                        new OrderReportHistoryAmount()
                        {
                            Id = Ulid.NewUlid().ToString(),
                            OrderYear = baseDate.AddMonths(2).Year,
                            OrderMonth = baseDate.AddMonths(2).Month,
                            ProspectiveSalesOrderAmount = 99m,
                            SalesOrderAmount = 999m
                        },
                    }
                },
            };

            // when
            var count = 0;
            List<OrderReportHistory> actual;
            using (var dbContext = new DesignDepartmentContext(configuration: _configuration!))
            {
                dbContext.OrderReportHistoryAmounts.RemoveRange(dbContext.OrderReportHistoryAmounts);
                dbContext.OrderReportHistories.RemoveRange(dbContext.OrderReportHistories);
                _ = dbContext.SaveChanges();
                dbContext.OrderReportHistories.AddRange(expected);
                count = dbContext.SaveChanges();
                actual = dbContext.OrderReportHistories.ToList();
            }

            // then
            Assert.AreEqual(expected.Count + expected.Sum(x => x.OrderReportHistoriesAmounts.Count), count);
            Assert.IsTrue(actual.Any());
            CollectionAssert.AreEquivalent(expected, actual);
            for (int i = 0; i < expected.Count; i++)
            {
                CollectionAssert.AreEquivalent(expected[i].OrderReportHistoriesAmounts.ToArray(), actual[i].OrderReportHistoriesAmounts.ToArray());
            }
        }
    }
}