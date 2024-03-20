using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wada.AchievementEntry.ViewModels;

namespace Wada.AchievementEntry.Models;

public class AchievementCollectionModel
{
    internal AchievementCollectionModel()
    { }

    internal AchievementCollectionModel(DateTime achievementDate,
                                        uint employeeNumber,
                                        string? employeeName,
                                        IEnumerable<IValidationErrorCollectionViewModel> validationResults)
    {
        AchievementDate.Value = achievementDate;
        EmployeeNumber.Value = employeeNumber;
        EmployeeName.Value = employeeName;
        ValidationResults.AddRange(validationResults);
    }

    public ReactivePropertySlim<bool> CheckedItem { get; } = new(true);

    public ReactivePropertySlim<DateTime> AchievementDate { get; } = new();

    public ReactivePropertySlim<uint> EmployeeNumber { get; } = new();

    public ReactivePropertySlim<string?> EmployeeName { get; } = new();

    public ReactiveCollection<IValidationErrorCollectionViewModel> ValidationResults { get; } = new();
}
