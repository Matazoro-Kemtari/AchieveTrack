using Wada.AchieveTrackService;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.AOP.Logging;
using Wada.VerifyWorkRecordApplication;

namespace Wada.VerifyAchievementRecordContentApplication;

public interface IVerifyWorkRecordUseCase
{
    Task<IEnumerable<IEnumerable<IValidationErrorResult>>> ExecuteAsync(IEnumerable<WorkRecordParam> achievementRecordParams);
}

public class VerifyWorkRecordUseCase : IVerifyWorkRecordUseCase
{
    private readonly IWorkRecordValidator _workRecordValidator;

    public VerifyWorkRecordUseCase(IWorkRecordValidator workRecordValidator)
    {
        _workRecordValidator = workRecordValidator;
    }

    [Logging]
    public async Task<IEnumerable<IEnumerable<IValidationErrorResult>>> ExecuteAsync(IEnumerable<WorkRecordParam> achievementRecordParams)
    {
        var parser = new Dictionary<Type, Func<IValidationError, IValidationErrorResult>>
        {
            { typeof(DuplicateWorkDateEmployeeError), DuplicateWorkDateEmployeeErrorResult.Parse },
            { typeof(InvalidWorkNumberError), InvalidWorkNumberErrorResult.Parse },
            { typeof(UnregisteredWorkNumberError), UnregisteredWorkNumberErrorResult.Parse },
            { typeof(WorkDateExpiredError), WorkDateExpiredErrorResult.Parse },
        };

        var results = await _workRecordValidator.ValidateWorkRecordsAsync(
            achievementRecordParams.Select(x => x.Convert()));
        return results.Select(x => x.Select(y=> parser[y.GetType()](y)));
    }
}