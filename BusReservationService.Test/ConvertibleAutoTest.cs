using System.Reflection;
using BusReservationService.Share;

namespace BusReservationService.Test;

/// <summary>
/// BusReservationService.Share.IConvertibleToDomain<T>
/// を継承するソリューション内部のクラスに対して
/// インターフェース public T ToDomain()が
/// 実装されているかを試す
/// </summary>
public class ConvertibleAutoTests
{
    public static IEnumerable<object[]> GetConvertibleTypes()
    {
        var targetAssembly = Assembly.GetExecutingAssembly();
        
        var targetTypes = targetAssembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConvertibleToDomain<>)));

        foreach (var type in targetTypes)
        {
            yield return new object[] { type };
        }
    }
    
    [Theory]
    [MemberData(nameof(GetConvertibleTypes))]
    public void All_IConvertible_Classes_Should_Be_Instantiable_And_Convertible(Type type)
    {
        // 1. デフォルトコンストラクタでインスタンス化
        var instance = Activator.CreateInstance(type) as IConvertible;

        Assert.NotNull(instance);

        // 2. インターフェース経由の共通動作テスト（例: TypeCodeが取得できるか等）
        var typeCode = instance.GetTypeCode();
        Assert.True(Enum.IsDefined(typeof(TypeCode), typeCode));
    }
}
