using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.DataBase.EFCore.DesignDepartment.Entities;

namespace Wada.DataBase.EFCore.DesignDepartment.Tests
{
    [TestClass()]
    public class OwnCompanyHolidaysTests
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
        public void 正常系_自社カレンダーのデータが取得できること()
        {
            // given
            // when
            List<OwnCompanyHoliday> actual;
            using (var dbContext = new DesignDepartmentContext(configuration: _configuration!))
            {
                actual = dbContext.OwnCompanyHolidays.ToList();
            }

            // then
            Assert.IsTrue(actual.Any());
        }
    }
}