using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.EmployeeAggregation;

namespace Wada.Data.OrderManagement.Tests
{
    [TestClass()]
    public class EmployeeReaderTests
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
        public async Task 正常系_社員情報が取得できること()
        {
            // given
            var employeeNumber = 4001u;

            // when
            IEmployeeRepository repository = new EmployeeRepository(_configuration!);
            var actual = await repository.FindByEmployeeNumberAsync(employeeNumber);

            // then
            Assert.IsNotNull(actual);
            var expected = TestEmployeeFactory.Create();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public async Task 異常系_社員に該当がない場合例外を返すこと()
        {
            // given
            var employeeNumber = 99999u;

            // when
            IEmployeeRepository repository = new EmployeeRepository(_configuration!);
            Task target() => _ = repository.FindByEmployeeNumberAsync(employeeNumber);

            // then
            var ex = await Assert.ThrowsExceptionAsync<EmployeeNotFoundException>(target);
            var message = $"社員情報が見つかりません 社員番号: {employeeNumber}";
            Assert.AreEqual(message, ex.Message);
        }
    }
}