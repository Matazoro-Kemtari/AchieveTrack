﻿using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.DataBase.EFCore.OrderManagement.Entities;

namespace Wada.DataBase.EFCore.OrderManagement.Tests
{
    [TestClass]
    public class EmployeeTests
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
        public void 正常系_社員のデーダが取得できること()
        {
            // given
            // when
            List<Employee> actual;
            using (var dbContext = new OrderManagementContext(_configuration!))
            {
                actual = dbContext.Employees.ToList();
            }

            // then
            Assert.IsTrue(actual.Any());
        }
    }
}