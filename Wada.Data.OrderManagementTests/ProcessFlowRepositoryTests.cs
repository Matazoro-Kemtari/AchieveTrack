using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ProcessFlowAggregation;

namespace Wada.Data.OrderManagement.Tests
{
    [TestClass()]
    public class ProcessFlowRepositoryTests
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
        public async Task 正常系_名前で実績工程が取得できること()
        {
            // given
            var name = "NC";
            var expected = 3u;

            // when
            IProcessFlowRepository repository = new ProcessFlowRepository(_configuration!);
            var actual = await repository.FindByNameAsync(name);

            // then
            Assert.AreEqual(expected, actual.Id);
            Assert.AreEqual(name, actual.Name);
        }

        [TestMethod()]
        public async Task 異常系_実績工程に該当がない場合例外を返すこと()
        {
            // given
            var name = "DUMMY";

            // when
            IProcessFlowRepository repository = new ProcessFlowRepository(_configuration!);
            Task target() => _ = repository.FindByNameAsync(name);

            // then
            var ex = await Assert.ThrowsExceptionAsync<ProcessFlowNotFoundException>(target);
            var message = $"実績工程が見つかりません 実績工程: {name}";
            Assert.AreEqual(message, ex.Message);
        }
    }
}