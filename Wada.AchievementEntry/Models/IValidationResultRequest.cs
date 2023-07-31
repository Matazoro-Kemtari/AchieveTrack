using System;
using Wada.AchieveTrackService.ValueObjects;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

public interface IValidationResultRequest : IValidationResultAttempt
{
}

public record class InvalidWorkNumberResultRequest : InvalidWorkNumberResultAttempt, IValidationResultRequest
{
    private InvalidWorkNumberResultRequest(WorkingNumber workingNumber, string jigCode, string note)
        : base(workingNumber, jigCode, note)
    { }

    private static new InvalidWorkNumberResultRequest Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static InvalidWorkNumberResultRequest Parse(IValidationResultAttempt validationResult)
    {
        if (validationResult is not InvalidWorkNumberResultAttempt)
            throw new ArgumentException(
                $"引数には{nameof(InvalidWorkNumberResultAttempt)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }

    public new string Message => "作業台帳に未登録の作業番号があります";
}

public record class DuplicateWorkDateEmployeeResultRequest : DuplicateWorkDateEmployeeResultAttempt, IValidationResultRequest
{
    private DuplicateWorkDateEmployeeResultRequest(WorkingNumber workingNumber, string jigCode, string note)
        : base(workingNumber, jigCode, note)
    { }

    private static new DuplicateWorkDateEmployeeResultRequest Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static DuplicateWorkDateEmployeeResultRequest Parse(IValidationResultAttempt validationResult)
    {
        if (validationResult is not DuplicateWorkDateEmployeeResultAttempt)
            throw new ArgumentException(
                $"引数には{nameof(DuplicateWorkDateEmployeeResultAttempt)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }
}

public record class UnregisteredWorkNumberResultRequest : UnregisteredWorkNumberResultAttempt, IValidationResultRequest
{
    private UnregisteredWorkNumberResultRequest(WorkingNumber workingNumber, string jigCode, string note)
        : base(workingNumber, jigCode, note)
    { }

    private static new UnregisteredWorkNumberResultRequest Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static UnregisteredWorkNumberResultRequest Parse(IValidationResultAttempt validationResult)
    {
        if (validationResult is not UnregisteredWorkNumberResultAttempt)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkNumberResultAttempt)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }

    // グルーピングされるのでふさわしいメッセージに書き換え
    public new string Message => "設計管理に未登録の作業番号があります";
}

public record class WorkDateExpiredResultRequest : WorkDateExpiredResultAttempt, IValidationResultRequest
{
    private WorkDateExpiredResultRequest(WorkingNumber workingNumber, string jigCode, string note)
        : base(workingNumber, jigCode, note)
    { }

    private static new WorkDateExpiredResultRequest Create(WorkingNumber workingNumber, string jigCode, string note)
        => new(workingNumber, jigCode, note);

    public static WorkDateExpiredResultRequest Parse(IValidationResultAttempt validationResult)
    {
        if (validationResult is not WorkDateExpiredResultAttempt)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredResultAttempt)}を渡してください",
                nameof(validationResult));

        return Create(validationResult.WorkingNumber, validationResult.JigCode, validationResult.Note);
    }

    // グルーピングされるのでふさわしいメッセージに書き換え
    public new string Message => "作業日が完成日を過ぎた作業番号があります";
}
