using Reactive.Bindings;
using System;
using System.Collections.Generic;
using Wada.AchievementEntry.ViewModels;

namespace Wada.AchievementEntry.Models;

public class AchievementCollectionModel
{
    public AchievementCollectionModel(DateTime achievementDate,
                                      uint employeeNumber,
                                      string? employeeName,
                                      IEnumerable<IValidationResultCollectionViewModel> validationResults)
    {
        AchievementDate.Value = achievementDate;
        EmployeeNumber.Value = employeeNumber;
        EmployeeName.Value = employeeName;
        ValidationResults.AddRangeOnScheduler(validationResults);
    }

    public ReactivePropertySlim<bool> CheckedItem { get; } = new(true);

    public ReactivePropertySlim<DateTime> AchievementDate { get; } = new();

    public ReactivePropertySlim<uint> EmployeeNumber { get; } = new();

    public ReactivePropertySlim<string?> EmployeeName { get; } = new();

    public ReactiveCollection<IValidationResultCollectionViewModel> ValidationResults { get; } = new();
}
