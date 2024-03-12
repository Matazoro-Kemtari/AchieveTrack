using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.DataBase.EFCore.DesignDepartment.Entities;

namespace Wada.DataBase.EFCore.DesignDepartment.Tests
{
    public partial class AttendancesTests
    {
        public partial class DepartmentCompanyHolidaysTests
        {
            [TestClass()]
            public class MatchedEmployeeNumbersTests
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
                public void 正常系_社員番号対応表のデータが取得できること()
                {
                    // given
                    // when
                    List<MatchedEmployeeNumber> actual;
                    using (var dbContext = new DesignDepartmentContext(_configuration!))
                    {
                        actual = dbContext.MatchedEmployeeNumbers.ToList();
                    }

                    // then
                    Assert.IsTrue(actual.Any());
                }
            }
        }
    }
}