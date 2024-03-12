namespace Wada.DataBase.EFCore.OrderManagement.ValueObjects;

public enum OrderStatus
{
    /// <summary>
    /// 見積のみ
    /// </summary>
    [NativeValue("見積のみ")]
    EstimateOnly,

    /// <summary>
    /// 失注
    /// </summary>
    [NativeValue("失注")]
    Lost,

    /// <summary>
    /// 完成
    /// </summary>
    [NativeValue("完成")]
    Completed,

    /// <summary>
    /// 検収
    /// </summary>
    [NativeValue("検収")]
    Inspection,

    /// <summary>
    /// 受注
    /// </summary>
    [NativeValue("受注")]
    OrderReceived,

    /// <summary>
    /// 参考見積
    /// </summary>
    [NativeValue("参考見積")]
    ReferenceEstimate,

    /// <summary>
    /// 請求
    /// </summary>
    [NativeValue("請求")]
    Invoiced,

    /// <summary>
    /// 納品
    /// </summary>
    [NativeValue("納品")]
    Delivered,

    /// <summary>
    /// 請求なし
    /// </summary>
    [NativeValue("請求なし")]
    NoInvoice,

    /// <summary>
    /// キャンセル
    /// </summary>
    [NativeValue("ｷｬﾝｾﾙ")]
    Canceled,
}

public static class OrderStatusEnumExtension
{
    public static T ThrowIf<T>(this T value, Func<T, bool> predicate, Exception exception)
        where T : Attribute
    {
        if (predicate(value)) throw exception;
        else return value;
    }

    public static string? GetNativeValue(this OrderStatus enumValue)
    {
        return enumValue.GetType()
            .GetField(enumValue.ToString()!)
            ?.GetCustomAttributes(typeof(NativeValueAttribute), false)
            .Cast<NativeValueAttribute>()
            .FirstOrDefault()
            ?.ThrowIf(a => a == null, new ArgumentException("属性が設定されていません"))
            .Name;
    }
}

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
public class NativeValueAttribute : Attribute
{
    /// <summary>表示名</summary>
    public string Name { get; set; }

    /// <summary>enum表示名属性</summary>
    /// <param name="name">表示名</param>
    public NativeValueAttribute(string name)
    {
        Name = name;
    }
}