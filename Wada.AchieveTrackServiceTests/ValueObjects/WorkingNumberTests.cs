using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackServiceTests.ValueObjects
{
    [TestClass()]
    public class WorkingNumberTests
    {
        [DataTestMethod]
        [DataRow("23A-1")]
        [DataRow("X23A-1")]
        public void 正常系_オブジェクト生成できること(string value)
        {
            // given
            // when
            var workingnumber = WorkingNumber.Create(value);

            // then
            Assert.IsNotNull(workingnumber);
        }

        [DataTestMethod]
        [DataRow("Z1A-1")]
        [DataRow("123A-1")]
        [DataRow("1A-12345")]
        [DataRow("1漢-1")]
        [DataRow("1a-1")]
        public void 異常系_形式不正な作業番号のときは例外を返すこと(string value)
        {
            // given
            // when
            void target() => _= WorkingNumber.Create(value);

            // then
            var ex = Assert.ThrowsException<Data.OrderManagement.Models.ValueObjects.WorkingNumberException>(target);
            var message = $"正しい作業番号の形式を入力してください 値: {value}";
            Assert.AreEqual(message, ex.Message);
        }
    }
}
