using System.Text.RegularExpressions;

namespace Wada.AchieveTrackService.ValueObjects;

public record class WorkingNumber
{
    private WorkingNumber(string value)
    {
        Value = Validate(value);
        Header = DivideHeader(value);
        Symbol = DivideSymbol(value);
        Number = DivideNumber(value);
    }

    public string Value { get; } 

    public string Header { get; }

    public string Symbol { get; }

    public uint Number { get; } 

    public override string ToString() => Value;


    public static WorkingNumber Create(string Value) => new(Value);

    private static string Validate(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (!Regex.IsMatch(value, @"^X?\d{1,2}[A-Z]-\d{1,4}$"))
            throw new WorkingNumberException(
                $"正しい作業番号の形式を入力してください 値: {value}");

        return value;
    }

    private static string DivideHeader(string value)
    {
        var match = Regex.Match(value, @"\d{1,2}[A-Z]");
        return match.Success ? match.Value : string.Empty;
    }

    private static string DivideSymbol(string value)
    {
        var match = Regex.Match(value, @"(?<=\d{1,2})[A-Z]");
        return match.Success ? match.Value : string.Empty;
    }

    private static uint DivideNumber(string value)
    {
        var match = Regex.Match(value, @"(?<=-)\d{1,4}");
        return match.Success ? uint.Parse(match.Value) : default;
    }
}

public class TestWorkingNumberFactory
{
    public static WorkingNumber Create(string value = "22Z-1")
        => WorkingNumber.Create(value);
}