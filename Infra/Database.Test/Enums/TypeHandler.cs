using Xunit;
using BusReservationService.Database.Enums;
using AccountStatusDomain = BusReservationService.Domain.ValueObject.AccountStatus;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Runtime.InteropServices;
using System.Data.Common;
using static Dapper.SqlMapper;
using Moq;
using BusReservationService.Share;
using Shouldly;

namespace BusReservationService.Database.Test.Enums;
public class TypeHandler_UnitTest
{
    readonly LowercaseEnumTypeHandler<AccountStatus> AccountStatusHandler = new();
    readonly LowercaseEnumTypeHandler<AccountRole> AccountRoleHandler = new();

    [Theory]
    [InlineData("just_created", AccountStatus.JustCreated)]
    [InlineData("pending_email_verify", AccountStatus.PendingEmailVerify)]
    [InlineData("on_boarding", AccountStatus.OnBoarding)]
    [InlineData("activated", AccountStatus.Activated)]
    [InlineData("suspended", AccountStatus.Suspended)]
    internal void Parse_ShouldBeFromSnakeCase_WithTypeHandler(string val, AccountStatus expected)
    {
        var result = AccountStatusHandler.Parse(val);
        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("just_created", AccountStatus.JustCreated)]
    [InlineData("pending_email_verify", AccountStatus.PendingEmailVerify)]
    [InlineData("on_boarding", AccountStatus.OnBoarding)]
    [InlineData("activated", AccountStatus.Activated)]
    [InlineData("suspended", AccountStatus.Suspended)]
    internal void SetValue_ShouldConvertValueAndType_WithoutMock(string expected, AccountStatus val)
    {
        var mockParam = new Mock<IDbDataParameter>();

        mockParam.SetupAllProperties();

        AccountStatusHandler.SetValue(mockParam.Object, val);

        mockParam.Object.Value.ShouldBe(expected);
        mockParam.Object.DbType.ShouldBe(DbType.String);
    }
}