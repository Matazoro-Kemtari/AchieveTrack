using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.DesignManagementAggregation;

namespace Wada.DataSource.OrderManagement.Tests
{
    [TestClass()]
    public class DesignManagementRepositoryTests
    {
        [TestMethod()]
        public void 正常系_設計管理に追加できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IDesignManagementRepository> designMock = new();
            designMock.Setup(x => x.Add(It.IsAny<Data.OrderManagement.Models.DesignManagementAggregation.DesignManagement>()))
                .Returns(1);

            // when
            IDesignManagementRepository repository = new DesignManagementRepository(designMock.Object);
            var param = TestDesignManagementFactory.Create();
            var actual = repository.Add(param);

            // then
            Assert.AreEqual(1, actual);
            designMock.Verify(x => x.Add(It.IsAny<Data.OrderManagement.Models.DesignManagementAggregation.DesignManagement>()), Times.Once);
        }

        [TestMethod()]
        public void 異常系_設計管理に追加できなかったとき例外を返すこと()
        {
            // given
            Mock<Data.OrderManagement.Models.IDesignManagementRepository> designMock = new();
            var designManagement = TestDesignManagementFactory.Create();
            var message = $"設計管理に登録できませんでした レコード: {designManagement}";
            designMock.Setup(x => x.Add(It.IsAny<Data.OrderManagement.Models.DesignManagementAggregation.DesignManagement>()))
                .Throws(new Data.OrderManagement.Models.DesignManagementAggregation.DesignManagementAggregationException(message));

            // when
            IDesignManagementRepository repository = new DesignManagementRepository(designMock.Object);
            void target() => repository.Add(designManagement);

            // then
            var ex = Assert.ThrowsException<DesignManagementAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod]
        public void 正常系_自社NOで設計管理が取得できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IDesignManagementRepository> designMock = new();
            var dbResponse = Data.OrderManagement.Models.DesignManagementAggregation.TestDesignManagementFactory.Create();
            designMock.Setup(x => x.FindByOwnCompanyNumber(dbResponse.OwnCompanyNumber))
                .Returns(dbResponse);

            // when
            IDesignManagementRepository repository = new DesignManagementRepository(designMock.Object);
            var actual = repository.FindByOwnCompanyNumber(dbResponse.OwnCompanyNumber);

            // then
            var expected = DesignManagement.Parse(dbResponse);
            Assert.AreEqual(expected.OwnCompanyNumber, actual.OwnCompanyNumber);
            Assert.AreEqual(expected.StartDate, actual.StartDate);
            Assert.AreEqual(expected.DesignLead, actual.DesignLead);
            designMock.Verify(x => x.FindByOwnCompanyNumber(It.IsAny<uint>()), Times.Once);
        }

        [TestMethod()]
        public void 異常系_自社NOに該当がなかったとき例外を返すこと()
        {
            // given
            Mock<Data.OrderManagement.Models.IDesignManagementRepository> designMock = new();
            var designManagement = TestDesignManagementFactory.Create();
            var message = $"自社NOを確認してください 設計管理に登録されていません 自社NO: {designManagement.OwnCompanyNumber}";
            designMock.Setup(x => x.FindByOwnCompanyNumber(designManagement.OwnCompanyNumber))
                .Throws(new Data.OrderManagement.Models.DesignManagementAggregation.DesignManagementAggregationException(message));

            // when
            IDesignManagementRepository repository = new DesignManagementRepository(designMock.Object);
            void target() => repository.FindByOwnCompanyNumber(designManagement.OwnCompanyNumber);

            // then
            var ex = Assert.ThrowsException<DesignManagementAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }

        [TestMethod]
        public async Task 正常系_非同期_自社NOで設計管理が取得できること()
        {
            // given
            Mock<Data.OrderManagement.Models.IDesignManagementRepository> designMock = new();
            var dbResponse = Data.OrderManagement.Models.DesignManagementAggregation.TestDesignManagementFactory.Create();
            designMock.Setup(x => x.FindByOwnCompanyNumberAsync(dbResponse.OwnCompanyNumber))
                .ReturnsAsync(dbResponse);

            // when
            IDesignManagementRepository repository = new DesignManagementRepository(designMock.Object);
            var actual = await repository.FindByOwnCompanyNumberAsync(dbResponse.OwnCompanyNumber);

            // then
            var expected = DesignManagement.Parse(dbResponse);
            Assert.AreEqual(expected.OwnCompanyNumber, actual.OwnCompanyNumber);
            Assert.AreEqual(expected.StartDate, actual.StartDate);
            Assert.AreEqual(expected.DesignLead, actual.DesignLead);
            designMock.Verify(x => x.FindByOwnCompanyNumberAsync(It.IsAny<uint>()), Times.Once);
        }

        [TestMethod()]
        public async Task 異常系_非同期_自社NOに該当がなかったとき例外を返すこと()
        {
            // given
            Mock<Data.OrderManagement.Models.IDesignManagementRepository> designMock = new();
            var designManagement = TestDesignManagementFactory.Create();
            var message = $"自社NOを確認してください 設計管理に登録されていません 自社NO: {designManagement.OwnCompanyNumber}";
            designMock.Setup(x => x.FindByOwnCompanyNumberAsync(designManagement.OwnCompanyNumber))
                .ThrowsAsync(new Data.OrderManagement.Models.DesignManagementAggregation.DesignManagementAggregationException(message));

            // when
            IDesignManagementRepository repository = new DesignManagementRepository(designMock.Object);
            Task target() => repository.FindByOwnCompanyNumberAsync(designManagement.OwnCompanyNumber);

            // then
            var ex = await Assert.ThrowsExceptionAsync<DesignManagementAggregationException>(target);
            Assert.AreEqual(message, ex.Message);
        }
    }
}