using Wada.AchieveTrackService;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.AOP.Logging;
using Wada.VerifyWorkRecordApplication;

namespace Wada.VerifyAchievementRecordContentApplication;

public interface IVerifyWorkRecordUseCase
{
    Task<IEnumerable<IEnumerable<IValidationResultAttempt>>> ExecuteAsync(IEnumerable<WorkRecordParam> achievementRecordParams);
}

public class VerifyWorkRecordUseCase : IVerifyWorkRecordUseCase
{
    private readonly IWorkRecordValidator _workRecordValidator;

    public VerifyWorkRecordUseCase(IWorkRecordValidator workRecordValidator)
    {
        _workRecordValidator = workRecordValidator;
    }

    [Logging]
    public async Task<IEnumerable<IEnumerable<IValidationResultAttempt>>> ExecuteAsync(IEnumerable<WorkRecordParam> achievementRecordParams)
    {
        var parser = new Dictionary<Type, Func<IValidationResult, IValidationResultAttempt>>
        {
            { typeof(DuplicateWorkDateEmployeeResult), DuplicateWorkDateEmployeeResultAttempt.Parse },
            { typeof(InvalidWorkNumberResult), InvalidWorkNumberResultAttempt.Parse },
            { typeof(UnregisteredWorkNumberResult), UnregisteredWorkNumberResultAttempt.Parse },
            { typeof(WorkDateExpiredResult), WorkDateExpiredResultAttempt.Parse },
        };

        var results = await _workRecordValidator.ValidateWorkRecordsAsync(
            achievementRecordParams.Select(x => x.Convert()));
        return results.Select(x => x.Select(y=> parser[y.GetType()](y)));
    }
}