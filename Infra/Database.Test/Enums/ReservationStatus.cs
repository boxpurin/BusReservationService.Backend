using Xunit;
using BusReservationService.Database.Enums;
using ReservationStatusDomain = BusReservationService.Domain.ValueObject.ReservationStatus.ReservationStatus;
using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Runtime.InteropServices;
using FluentAssertions;

namespace BusReservationService.Database.Test.Enums;

public class ReservationStatus_UnitTest
{

    /// <summary>
    /// Database.Enums.AccountStatus から
    /// Domain.ValueObject.AccountStatus への
    /// ToDomain によって、同じ名前の値への変換を検証する
    /// </summary>
    [Fact]
    public void ToDomain_ShouldBeDomain_WithSameFieldName()
    {
        var tableVal = Enum.GetValues<ReservationStatus>();
        var domainNames = Enum.GetNames<ReservationStatusDomain>();
        var zipped = tableVal.Zip(domainNames);

        foreach (var (val, name) in zipped){
            if(Enum.TryParse<ReservationStatusDomain>(name, false, out var domain))
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
        var tableVal = Enum.GetValues<ReservationStatus>();
        var tableNames = Enum.GetNames<ReservationStatus>();
        var zipped = tableVal.Zip(tableNames);

        foreach (var (val, name) in zipped){
            var domain = Enum.Parse<ReservationStatusDomain>(name);
            val.ToDomain().Should().Be(domain);            
        } 
    }
}
