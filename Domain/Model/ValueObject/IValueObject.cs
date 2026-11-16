using System.Text.Json;
using System.Text.Json.Serialization;

namespace BusReservationService.Domain.ValueObject;

interface IValueObject<TSelf, V> where TSelf : IValueObject<TSelf, V>
{
    V Value { get; }
    static abstract TSelf Create(V value);
}

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