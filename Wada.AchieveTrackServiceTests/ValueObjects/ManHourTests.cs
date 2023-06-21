using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackServiceTests.ValueObjects
{
    [TestClass()]
    public class ManHourTests
    {
        [TestMethod()]
        public void 正常系_オブジェクト生成できること()
        {
            // given
            // when
            var manHour = ManHour.Create(5.55m);

            // then
            Assert.IsNotNull(manHour);
        }

        [DataTestMethod()]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(0.01)]
        public void 異常系_コンマ02より小さい工数のとき例外を返すこと(double value)
        {
            // given
            const decimal minimumManHour = 0.02m;

            // when
            void target() => _ = ManHour.Create((decimal)value);

            // then
            var ex = Assert.ThrowsException<DomainException>(target);
            var message = $"工数は最小値({minimumManHour:F2})より大きい値にしてください 工数: {value:F2}";
            Assert.AreEqual(message, ex.Message);
        }
    }
}
