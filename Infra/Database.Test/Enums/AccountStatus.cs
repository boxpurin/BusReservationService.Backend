using Xunit;
using BusReservationService.Database.Enums;
using AccountStatusDomain = BusReservationService.Domain.ValueObject.AccountStatus.AccountStatus;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Runtime.InteropServices;
using Shouldly;

namespace BusReservationService.Database.Test.Enums;

public class AccountStatus_UnitTest
{
    /// <summary>
    /// Database.Enums.AccountStatus から
    /// Domain.ValueObject.AccountStatus への
    /// ToDomain によって、同じ名前の値への変換を検証する
    /// </summary>
    [Fact]
    public void ToDomain_ShouldBeDomain_WithSameFieldName()
    {
        var tableVal = Enum.GetValues<AccountStatus>();
        var domainNames = Enum.GetNames<AccountStatusDomain>();
        var zipped = tableVal.Zip(domainNames);

        foreach (var (val, name) in zipped){
            if(Enum.TryParse<AccountStatusDomain>(name, false, out var domain))
            {
                val.ToDomain().ShouldBe(domain);
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
        var tableVal = Enum.GetValues<AccountStatus>();
        var tableNames = Enum.GetNames<AccountStatus>();
        var zipped = tableVal.Zip(tableNames);

        foreach (var (val, name) in zipped){
            var domain = Enum.Parse<AccountStatusDomain>(name);
            val.ToDomain().ShouldBe(domain);            
        } 
    }
}