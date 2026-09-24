using Dapper;
using System.Data;

public class LowercaseEnumTypeHandler<T> : SqlMapper.TypeHandler<T> where T : struct, Enum
{
    // DBへの書き込み時 (C# -> SQL)
    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = value.ToString().ToLowerInvariant();
        parameter.DbType = DbType.String;
    }

    // DBからの読み込み時 (SQL -> C#)
    public override T Parse(object value)
    {
        if (value is string str && Enum.TryParse<T>(str, true, out var result))
        {
            return result;
        }
        if (value != null && Enum.TryParse<T>(value.ToString(), true, out var parsed))
        {
            return parsed;
        }
        return default;
    }
}