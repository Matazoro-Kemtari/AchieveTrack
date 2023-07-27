﻿using Wada.AchieveTrackService.ValueObjects;

namespace Wada.AchieveTrackService.WorkRecordValidator;

/// <summary>
/// 作業日と社員NOの組み合わせが既に存在する結果
/// </summary>
public record class DuplicateWorkDateEmployeeResult : IValidationResult
{
    protected DuplicateWorkDateEmployeeResult(WorkingNumber workingNumber, string note)
    {
        WorkingNumber = workingNumber;
        Note = note;
    }

    public static DuplicateWorkDateEmployeeResult Create(WorkingNumber workingNumber, string note) => new(workingNumber, note);

    public string Message => "この作業日と社員番号の組み合わせが 実績処理で既に存在します";

    public WorkingNumber WorkingNumber { get; }

    public string Note { get; }
}

public class TestDuplicateWorkDateEmployeeResultFactory
{
    public static DuplicateWorkDateEmployeeResult Create(WorkingNumber? workingNumber = default,
                                                      string note = "特記事項")
    {
        workingNumber ??= TestWorkingNumberFactory.Create();
        return DuplicateWorkDateEmployeeResult.Create(workingNumber, note);
    }
}
