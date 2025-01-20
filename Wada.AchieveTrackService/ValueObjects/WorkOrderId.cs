using System.Text.RegularExpressions;

namespace Wada.AchieveTrackService.ValueObjects;

public partial record class WorkOrderId
{
    private WorkOrderId(string value)
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


    public static WorkOrderId Create(string Value) => new(Value);

    private static string Validate(string value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        if (!WorkOrderIdRegex().IsMatch(value))
            throw new WorkOrderIdException(
                $"正しい作業番号の形式を入力してください 値: {value}");

        return value;
    }

    private static string DivideHeader(string value)
    {
        var match = HeaderRegex().Match(value);
        return match.Success ? match.Value : string.Empty;
    }

    private static string DivideSymbol(string value)
    {
        var match = SymbolRegex().Match(value);
        return match.Success ? match.Value : string.Empty;
    }

    private static uint DivideNumber(string value)
    {
        var match = NumberRegex().Match(value);
        return match.Success ? uint.Parse(match.Value) : default;
    }

    [GeneratedRegex(@"^X?\d{1,2}[A-Z]-\d{1,4}$")]
    private static partial Regex WorkOrderIdRegex();
    [GeneratedRegex(@"\d{1,2}[A-Z]")]
    private static partial Regex HeaderRegex();
    [GeneratedRegex(@"(?<=\d{1,2})[A-Z]")]
    private static partial Regex SymbolRegex();
    [GeneratedRegex(@"(?<=-)\d{1,4}")]
    private static partial Regex NumberRegex();
}

public class TestWorkOrderIdFactory
{
    public static WorkOrderId Create(string value = "22Z-1")
        => WorkOrderId.Create(value);
}