﻿using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業台帳にない作業番号の結果
/// </summary>
public record class InvalidWorkNumberResult : IValidationResult
{
    protected InvalidWorkNumberResult(WorkingNumber workingNumber, string jigCode, string note)
    {
        WorkingNumber = workingNumber;
        JigCode = jigCode;
        Note = note;
    }

    public static InvalidWorkNumberResult Create(WorkingNumber workingNumber, string jigCode, string note) => new(workingNumber, jigCode, note);

    public string Message => "作業台帳にない作業番号です";

    public WorkingNumber WorkingNumber { get; }

    public string JigCode { get; }

    public string Note { get; }
}

public class TestInvalidWorkNumberResultFactory
{
    public static InvalidWorkNumberResult Create(WorkingNumber? workingNumber = default,
                                                 string jigCode = "11A",
                                                 string note = "特記事項")
    {
        workingNumber ??= TestWorkingNumberFactory.Create();
        return InvalidWorkNumberResult.Create(workingNumber, jigCode, note);
    }
}
