using System.Text.Json;
using System.Text.Json.Serialization;

namespace BusReservationService.Domain.ValueObject;

/// <summary>
/// 値オブジェクトのためのインターフェース
/// </summary>
/// <typeparam name="TSelf">自身の型</typeparam>
/// <typeparam name="TValue">値の型</typeparam>
interface IValueObject<TSelf, TValue> where TSelf : IValueObject<TSelf, TValue>
{
    /// <summary>
    /// 値オブジェクトが保持する値
    /// </summary>
    TValue Value { get; init; }

    /// <summary>
    /// 値から値オブジェクトを生成する
    /// </summary>
    /// <param name="value">値</param>
    /// <returns>生成された値オブジェクト</returns>
    static abstract TSelf Create(TValue value);
}

/// <summary>
/// 値オブジェクトの JSON 変換を行うコンバーター
/// </summary>
/// <typeparam name="TSelf">自身の型</typeparam>
/// <typeparam name="TValue">値の型</typeparam>
internal sealed class ValueObjectJsonConverter<TSelf, TValue> : JsonConverter<TSelf>
    where TSelf : IValueObject<TSelf, TValue>
{
    public override TSelf Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = JsonSerializer.Deserialize<TValue>(ref reader, options);
        return TSelf.Create(value);
    }
    public override void Write(Utf8JsonWriter writer, TSelf value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value.Value, options);
    }
}