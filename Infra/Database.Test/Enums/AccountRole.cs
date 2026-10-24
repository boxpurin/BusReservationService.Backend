using Xunit;
using BusReservationService.Database.Enums;
using AccountRoleDomain = BusReservationService.Domain.ValueObject.AccountRole.AccountRole;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Runtime.InteropServices;
using FluentAssertions;

namespace BusReservationService.Database.Test.Enums;

public class AccountRole_UnitTest
{
    /// <summary>
    /// Database.Enums.AccountStatus から
    /// Domain.ValueObject.AccountStatus への
    /// ToDomain によって、同じ名前の値への変換を検証する
    /// </summary>
    [Fact]
    public void ToDomain_ShouldBeDomain_WithSameFieldName()
    {
        var tableVal = Enum.GetValues<AccountRole>();
        var domainNames = Enum.GetNames<AccountRoleDomain>();
        var zipped = tableVal.Zip(domainNames);

        foreach (var (val, name) in zipped){
            if(Enum.TryParse<AccountRoleDomain>(name, false, out var domain))
            {
                val.ToDomain().Should().Be(domain);
            }
        } 
    }

    /// <summary>
    /// Database.Enums.AccountStatus の全ての値から
    /// Domain.ValueObject.AccountStatus への
    /// ToDomain によって、同じ名前の値への変換を保証する
    /// </summary>
    [Fact]
    public void AllValue_ShouldBeConvertToDomain()
    {
        var tableVal = Enum.GetValues<AccountRole>();
        var tableNames = Enum.GetNames<AccountRole>();
        var zipped = tableVal.Zip(tableNames);

        foreach (var (val, name) in zipped){
            var domain = Enum.Parse<AccountRoleDomain>(name);
            val.ToDomain().Should().Be(domain);            
        }
    }
}