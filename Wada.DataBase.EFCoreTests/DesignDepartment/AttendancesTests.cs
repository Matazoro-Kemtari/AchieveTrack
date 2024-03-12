using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.DataBase.EFCore.DesignDepartment.Entities;

namespace Wada.DataBase.EFCore.DesignDepartment.Tests
{
    [TestClass()]
    public partial class AttendancesTests
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
        public void 正常系_勤怠のデータが取得できること()
        {
            // given
            // when
            List<Attendance> actual;
            using (var dbContext = new DesignDepartmentContext(_configuration!))
            {
                actual = dbContext.Attendances.Where(x => x.Id == "01GS9DCETEG9WXXTC2CX2T0WHX")
                                              .Include(x => x.Achievements)
                                              .ToList();
            }

            // then
            Assert.IsTrue(actual.Any());
            Assert.IsTrue(actual.First().Achievements.Any());
        }
    }
}
