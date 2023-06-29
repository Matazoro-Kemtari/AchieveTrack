using System;
using Wada.VerifyAchievementRecordContentApplication;
using Wada.VerifyWorkRecordApplication;

namespace Wada.AchievementEntry.Models;

public interface IValidationResultRequest : IValidationResultAttempt
{
}

public record class InvalidWorkNumberResultRequest : InvalidWorkNumberResultAttempt, IValidationResultRequest
{
    private InvalidWorkNumberResultRequest() { }

    private static new InvalidWorkNumberResultRequest Create() => new();

    public static InvalidWorkNumberResultRequest Parse(IValidationResultAttempt validationResult)
    {
        if (validationResult is not InvalidWorkNumberResultAttempt)
            throw new ArgumentException(
                $"引数には{nameof(InvalidWorkNumberResultAttempt)}を渡してください",
                nameof(validationResult));

        return Create();
    }

    // グルーピングされるのでふさわしいメッセージに書き換え
    public new string Message => "作業台帳に未登録の作業番号があります";
}

public record class DuplicateWorkDateEmployeeResultRequest : DuplicateWorkDateEmployeeResultAttempt, IValidationResultRequest
{
    private DuplicateWorkDateEmployeeResultRequest() { }

    private static new DuplicateWorkDateEmployeeResultRequest Create() => new();

    public static DuplicateWorkDateEmployeeResultRequest Parse(IValidationResultAttempt validationResult)
    {
        if (validationResult is not DuplicateWorkDateEmployeeResultAttempt)
            throw new ArgumentException(
                $"引数には{nameof(DuplicateWorkDateEmployeeResultAttempt)}を渡してください",
                nameof(validationResult));

        return Create();
    }
}

public record class UnregisteredWorkNumberResultRequest : UnregisteredWorkNumberResultAttempt, IValidationResultRequest
{
    private UnregisteredWorkNumberResultRequest() { }

    private static new UnregisteredWorkNumberResultRequest Create() => new();

    public static UnregisteredWorkNumberResultRequest Parse(IValidationResultAttempt validationResult)
    {
        if (validationResult is not UnregisteredWorkNumberResultAttempt)
            throw new ArgumentException(
                $"引数には{nameof(UnregisteredWorkNumberResultAttempt)}を渡してください",
                nameof(validationResult));

        return Create();
    }

    // グルーピングされるのでふさわしいメッセージに書き換え
    public new string Message => "設計管理に未登録の作業番号があります";
}

public record class WorkDateExpiredResultRequest : WorkDateExpiredResultAttempt, IValidationResultRequest
{
    private WorkDateExpiredResultRequest() { }

    private static new WorkDateExpiredResultRequest Create() => new();

    public static WorkDateExpiredResultRequest Parse(IValidationResultAttempt validationResult)
    {
        if (validationResult is not WorkDateExpiredResultAttempt)
            throw new ArgumentException(
                $"引数には{nameof(WorkDateExpiredResultAttempt)}を渡してください",
                nameof(validationResult));

        return Create();
    }

    // グルーピングされるのでふさわしいメッセージに書き換え
    public new string Message => "作業日が完成日を過ぎた作業番号があります";
}
