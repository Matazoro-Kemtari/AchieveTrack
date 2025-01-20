using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkOrderAggregation;

namespace Wada.Data.OrderManagement.Tests
{
    [TestClass()]
    public class WorkOrderRepositoryTests
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
        public async Task 正常系_作業台帳が取得できること()
        {
            // given
            var workOrderId = "23K-110";

            // when
            IWorkOrderRepository repository = new WorkOrderRepository(_configuration!);
            var actual = await repository.FindByWorkOrderIdAsync(WorkOrderId.Create(workOrderId));

            // then
            Assert.IsNotNull(actual);
            Assert.AreEqual(workOrderId, actual.WorkOrderId.Value);
        }

        [TestMethod()]
        public async Task 異常系_作業台帳に作業番号がない場合例外を返すこと()
        {
            // given
            var workOrderId = "99Q-999";

            // when
            IWorkOrderRepository repository = new WorkOrderRepository(_configuration!);
            Task target() => repository.FindByWorkOrderIdAsync(WorkOrderId.Create(workOrderId));

            // then
            var ex = await Assert.ThrowsExceptionAsync<WorkOrderNotFoundException>(target);
            var message = $"作業番号を確認してください 受注管理に登録されていません 作業番号: {workOrderId}";
            Assert.AreEqual(message, ex.Message);
        }
    }
}