using Livet.Messaging;
using Livet.Messaging.IO;
using System.Windows;
using Wada.AOP.Logging;

namespace Wada.AchievementEntry.ViewModels;

public class MessageNotificationViaLivet
{
    [Logging]
    public static InformationMessage MakeErrorMessage(string message, string title = "実績登録") => new(
        message, title, MessageBoxImage.Error, "Info");

    [Logging]
    public static InformationMessage MakeExclamationMessage(string message, string title = "実績登録") => new(
        message, title, MessageBoxImage.Exclamation, "Info");

    [Logging]
    public static InformationMessage MakeInformationMessage(string message, string title = "実績登録") => new(
        message, title, MessageBoxImage.Information, "Info");

    [Logging]
    public static ConfirmationMessage MakeQuestionMessage(string message, string title = "実績登録") => new(
        message, title, MessageBoxImage.Question, "Confirm")
    { Button = MessageBoxButton.YesNo };

    [Logging]
    public static SavingFileSelectionMessage MakeSaveFileDialog() => new("SaveFiling");
}
