using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService.EmployeeAggregation;
using Wada.Data.OrderManagement.Models;
using Wada.WriteWorkRecordApplication;

namespace Wada.DataSource.OrderManagement.Tests
{
    [TestClass()]
    public class EmployeeReaderTests
    {
        [TestMethod()]
        public async Task 正常系_社員情報が取得できること()
        {
            // given
            Mock<IEmployeeRepository> employeeMock = new();
            var employee = Data.OrderManagement.Models.EmployeeAggregation.TestEmployeeFactory.Create();
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ReturnsAsync(employee);

            // when
            IEmployeeReader reader = new EmployeeReader(employeeMock.Object);
            var actual = await reader.FindByEmployeeNumberAsync(employee.EmployeeNumber);

            // then
            Assert.IsNotNull(actual);
            var expected = Employee.Parse(employee);
            Assert.AreEqual(expected, actual);
            employeeMock.Verify(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()), Times.Once());
        }

        [TestMethod()]
        public async Task 異常系_社員に該当がない場合例外を返すこと()
        {
            // given
            Mock<IEmployeeRepository> employeeMock = new();
            var employee = Data.OrderManagement.Models.EmployeeAggregation.TestEmployeeFactory.Create();
            var message = $"社員番号を確認してください 受注管理に登録されていません 社員番号: {employee!.EmployeeNumber}";
            employeeMock.Setup(x => x.FindByEmployeeNumberAsync(It.IsAny<uint>()))
                .ThrowsAsync(new Data.OrderManagement.Models.EmployeeAggregation.EmployeeAggregationException(message));

            // when
            IEmployeeReader reader = new EmployeeReader(employeeMock.Object);
            Task target() => reader.FindByEmployeeNumberAsync(employee!.EmployeeNumber);

            // then
            var ex = await Assert.ThrowsExceptionAsync<EmployeeAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }
    }
}