using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;
using Wada.AchieveTrackService.WorkingLedgerAggregation;

namespace Wada.Data.OrderManagement.Tests
{
    [TestClass()]
    public class WorkingLedgerRepositoryTests
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
            var workingNumber = "23K-110";

            // when
            IWorkingLedgerRepository repository = new WorkingLedgerRepository(_configuration!);
            var actual = await repository.FindByWorkingNumberAsync(WorkingNumber.Create(workingNumber));

            // then
            Assert.IsNotNull(actual);
            Assert.AreEqual(workingNumber, actual.WorkingNumber.Value);
        }

        [TestMethod()]
        public async Task 異常系_作業台帳に作業番号がない場合例外を返すこと()
        {
            // given
            var workingNumber = "99Q-999";

            // when
            IWorkingLedgerRepository repository = new WorkingLedgerRepository(_configuration!);
            Task target() => repository.FindByWorkingNumberAsync(WorkingNumber.Create(workingNumber));

            // then
            var ex = await Assert.ThrowsExceptionAsync<WorkingLedgerNotFoundException>(target);
            var message = $"作業番号を確認してください 受注管理に登録されていません 作業番号: {workingNumber}";
            Assert.AreEqual(message, ex.Message);
        }
    }
}