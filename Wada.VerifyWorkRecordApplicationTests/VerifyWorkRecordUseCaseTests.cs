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
            IValidationResult[] validationResults = new IValidationResult[]
            {
                ValidationSuccessResult.Create(),
                ValidationSuccessResult.Create(),
                ValidationSuccessResult.Create(),
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
            Assert.IsTrue(actual.All(x => x.GetType() == typeof(ValidationSuccessResultAttempt)));
        }

        [TestMethod()]
        public async Task 正常系_作業日が完成を過ぎている作業日報が検出できること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationResult[] validationResults = new IValidationResult[]
            {
                ValidationSuccessResult.Create(),
                WorkDateExpiredResult.Create(),
                ValidationSuccessResult.Create(),
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
            Assert.IsTrue(actual.Any(x => x.GetType() == typeof(WorkDateExpiredResultAttempt)));
        }

        [TestMethod()]
        public async Task 正常系_作業台帳にない作業番号の作業日報が検出できること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationResult[] validationResults = new IValidationResult[]
            {
                ValidationSuccessResult.Create(),
                InvalidWorkNumberResult.Create(),
                ValidationSuccessResult.Create(),
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
            Assert.IsTrue(actual.Any(x => x.GetType() == typeof(InvalidWorkNumberResultAttempt)));
        }

        [TestMethod()]
        public async Task 正常系_作業日と社員NOの組み合わせが既に存在する作業日報が検出できること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationResult[] validationResults = new IValidationResult[]
            {
                ValidationSuccessResult.Create(),
                DuplicateWorkDateEmployeeResult.Create(),
                ValidationSuccessResult.Create(),
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
            Assert.IsTrue(actual.Any(x => x.GetType() == typeof(DuplicateWorkDateEmployeeResultAttempt)));
        }

        [TestMethod()]
        public async Task 正常系_設計管理に未登録の作業番号の作業日報が検出できること()
        {
            // given
            Mock<IWorkRecordValidator> validatorMock = new();
            IValidationResult[] validationResults = new IValidationResult[]
            {
                ValidationSuccessResult.Create(),
                UnregisteredWorkNumberResult.Create(),
                ValidationSuccessResult.Create(),
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
            Assert.IsTrue(actual.Any(x => x.GetType() == typeof(UnregisteredWorkNumberResultAttempt)));
        }
    }
}