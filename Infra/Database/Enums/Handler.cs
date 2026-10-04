using Dapper;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace BusReservationService.Database.Enums;

internal class LowercaseEnumTypeHandler<T> : SqlMapper.TypeHandler<T> where T : struct, Enum
{
    // DBへの書き込み時 (C# -> SQL)
    public override void SetValue(IDbDataParameter parameter, T value)
    { 
        parameter.Value = Regex.Replace(value.ToString(), "(?<!^)([A-Z])", "_$1").ToLower();
        Console.WriteLine(parameter.Value);
        parameter.DbType = DbType.String;
    }

    // DBからの読み込み時 (SQL -> C#)
    public override T Parse(object value)
    {
        if (value is string str && Enum.TryParse<T>(ToUpperCamelCase(str), true, out var result))
        {
            return result;
        }
        if (value != null && Enum.TryParse<T>(ToUpperCamelCase(value.ToString()!), true, out var parsed))
        {
            return parsed;
        }
        return default;
    }

    private static string ToUpperCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) { return input; }

        // アンダースコアで分割し、各単語の先頭を大文字にして結合する
        return string.Concat(
            input.Split('_')
                 .Select(word => word.Length > 0 ? char.ToUpper(word[0]) + word.Substring(1) : "")
        );
    }
}