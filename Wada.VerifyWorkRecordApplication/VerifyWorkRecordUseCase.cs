using Wada.AchieveTrackService;
using Wada.AchieveTrackService.WorkRecordValidator;
using Wada.VerifyWorkRecordApplication;

namespace Wada.VerifyAchievementRecordContentApplication;

public interface IVerifyWorkRecordUseCase
{
    Task<IEnumerable<IValidationResultAttempt>> ExecuteAsync(IEnumerable<WorkRecordParam> achievementRecordParams);
}

public class VerifyWorkRecordUseCase : IVerifyWorkRecordUseCase
{
    private readonly IWorkRecordValidator _workRecordValidator;

    public VerifyWorkRecordUseCase(IWorkRecordValidator workRecordValidator)
    {
        _workRecordValidator = workRecordValidator;
    }

    public async Task<IEnumerable<IValidationResultAttempt>> ExecuteAsync(IEnumerable<WorkRecordParam> achievementRecordParams)
    {
        var parser = new Dictionary<Type, Func<IValidationResult, IValidationResultAttempt>>
        {
            { typeof(ValidationSuccessResult), ValidationSuccessResultAttempt.Parse },
            { typeof(DuplicateWorkDateEmployeeResult), DuplicateWorkDateEmployeeResultAttempt.Parse },
            { typeof(InvalidWorkNumberResult), InvalidWorkNumberResultAttempt.Parse },
            { typeof(UnregisteredWorkNumberResult), UnregisteredWorkNumberResultAttempt.Parse },
            { typeof(WorkDateExpiredResult), WorkDateExpiredResultAttempt.Parse },
        };

        var results = await _workRecordValidator.ValidateWorkRecords(achievementRecordParams.Select(x => x.ConvertWorkRecord()));
        return results.Select(x => parser[x.GetType()](x));
    }
}