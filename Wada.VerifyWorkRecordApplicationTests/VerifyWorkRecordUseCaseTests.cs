using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Wada.AchieveTrackService;
using Wada.AchieveTrackService.WorkRecordReader;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyWorkRecordApplication;

namespace Wada.VerifyAchievementRecordContentApplication.Tests
{
    [TestClass()]
    public class VerifyWorkRecordUseCaseTests
    {
        [TestMethod()]
        public async Task 正常系_作業日報の検証が通ること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationError[][] validationResults = Array.Empty<IValidationError[]>();
            validatorMock.Setup(x => x.ValidateWorkRecordsAsync(It.IsAny<IEnumerable<WorkRecord>>()))
                .ReturnsAsync(validationResults);

            var param = new WorkRecordParam[]
            {
                TestAchievementRecordParamFactory.Create(),
            };

            // when
            IVerifyWorkRecordUseCase useCase = new VerifyWorkRecordUseCase(validatorMock.Object);
            var actual = await useCase.ExecuteAsync(param);

            // then
            Assert.AreEqual(validationResults.Length, actual.Count());
            Assert.IsFalse(actual.SelectMany(x => x).Any());
        }

        [TestMethod()]
        public async Task 正常系_作業日が完成を過ぎている作業日報が検出できること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationError[][] validationResults = new IValidationError[][]
            {
                new IValidationError[]
                {
                    TestWorkDateExpiredResultFactory.Create(),
                },
            };
            validatorMock.Setup(x => x.ValidateWorkRecordsAsync(It.IsAny<IEnumerable<WorkRecord>>()))
                .ReturnsAsync(validationResults);

            var param = new WorkRecordParam[]
            {
                TestAchievementRecordParamFactory.Create(),
            };

            // when
            IVerifyWorkRecordUseCase useCase = new VerifyWorkRecordUseCase(validatorMock.Object);
            var actual = await useCase.ExecuteAsync(param);

            // then
            Assert.AreEqual(validationResults.Length, actual.Count());
            Assert.IsTrue(actual.SelectMany(x => x).Any(x => x.GetType() == typeof(WorkDateExpiredErrorResult)));
        }

        [TestMethod()]
        public async Task 正常系_作業台帳にない作業番号の作業日報が検出できること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationError[][] validationResults = new IValidationError[][]
            {
                new IValidationError[]
                {
                    TestInvalidWorkNumberResultFactory.Create(),
                },
            };
            validatorMock.Setup(x => x.ValidateWorkRecordsAsync(It.IsAny<IEnumerable<WorkRecord>>()))
                .ReturnsAsync(validationResults);

            var param = new WorkRecordParam[]
            {
                TestAchievementRecordParamFactory.Create(),
            };

            // when
            IVerifyWorkRecordUseCase useCase = new VerifyWorkRecordUseCase(validatorMock.Object);
            var actual = await useCase.ExecuteAsync(param);

            // then
            Assert.AreEqual(validationResults.Length, actual.Count());
            Assert.IsTrue(actual.SelectMany(x => x).Any(x => x.GetType() == typeof(InvalidWorkNumberErrorResult)));
        }

        [TestMethod()]
        public async Task 正常系_作業日と社員NOの組み合わせが既に存在する作業日報が検出できること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationError[][] validationResults = new IValidationError[][]
            {
                new IValidationError[]
                {
                    TestDuplicateWorkDateEmployeeResultFactory.Create(),
                },
            };
            validatorMock.Setup(x => x.ValidateWorkRecordsAsync(It.IsAny<IEnumerable<WorkRecord>>()))
                .ReturnsAsync(validationResults);

            var param = new WorkRecordParam[]
            {
                TestAchievementRecordParamFactory.Create(),
            };

            // when
            IVerifyWorkRecordUseCase useCase = new VerifyWorkRecordUseCase(validatorMock.Object);
            var actual = await useCase.ExecuteAsync(param);

            // then
            Assert.AreEqual(validationResults.Length, actual.Count());
            Assert.IsTrue(actual.SelectMany(x => x).Any(x => x.GetType() == typeof(DuplicateWorkDateEmployeeErrorResult)));
        }

        [TestMethod()]
        public async Task 正常系_設計管理に未登録の作業番号の作業日報が検出できること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationError[][] validationResults = new IValidationError[][]
            {
                new IValidationError[]
                {
                    TestUnregisteredWorkOrderIdResultFactory.Create(),
                },
            };
            validatorMock.Setup(x => x.ValidateWorkRecordsAsync(It.IsAny<IEnumerable<WorkRecord>>()))
                .ReturnsAsync(validationResults);

            var param = new WorkRecordParam[]
            {
                TestAchievementRecordParamFactory.Create(),
            };

            // when
            IVerifyWorkRecordUseCase useCase = new VerifyWorkRecordUseCase(validatorMock.Object);
            var actual = await useCase.ExecuteAsync(param);

            // then
            Assert.AreEqual(validationResults.Length, actual.Count());
            Assert.IsTrue(actual.SelectMany(x => x).Any(x => x.GetType() == typeof(UnregisteredWorkOrderIdErrorResult)));
        }
    }
}