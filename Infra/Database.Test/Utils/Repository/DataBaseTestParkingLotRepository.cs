using BusReservationService.Database.Interface;
using BusReservationService.Database.Repository;
using BusReservationService.Domain.Entity;
using BusReservationService.Database.Table;
using Dapper;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.Prefecture;

namespace BusReservationService.Infra.Database.Test.Utils.Repository;

/// <summary>
/// テストケースで使用するヘルパーリポジトリ
/// テストケースのための追加、更新、全件取得、削除を行うリポジトリ
/// </summary>
/// <param name="context">接続セッション</param>
internal class DataBaseTestParkingLotRepository(IDbContext context)
{

    public async Task<ParkingLotEntity> AddAsync(
        string parkingLotName,
        string zipCode = "0000000",
        Prefecture prefecture = Prefecture.Tokyo,
        string streetAddress = "",
        string phoneNumber = "000-0000-0000",
        string faxNumber = "000-0000-0000",
        short maxCapacity = 5,
        short reservationCapacity = 3,
        bool isAllowsBoardingAlighting = false,
        bool isActive = true)
    {
        context.EnsureConnectionOpen();
        var sql = """
            INSERT INTO parking_lots(
                display_name, prefecture_id, zip_code, street_address, phone_number, fax_number,
                max_capacity, reservation_capacity, is_allows_boarding_alighting, is_active
            )
            VALUES (
                @ParkingLotName, @Prefecture, @ZipCode, @StreetAddress, @PhoneNumber, @FaxNumber,
                @MaxCapacity, @ReservationCapacity, @IsAllowsBoardingAlighting, @IsActive
            )
            RETURNING * ;
        """;

        var entity = ParkingLotEntity.CreateNewParkingLotEntity(
            parkingLotName,
            zipCode,
            prefecture,
            streetAddress,
            phoneNumber,
            faxNumber,
            maxCapacity,
            reservationCapacity,
            isAllowsBoardingAlighting,
            isActive
        );

        var table = await context.Connection.QueryFirstAsync<ParkingLotTable>(
            sql, entity, context.Transaction
        );

        return table.ToDomain();
    }
    
    public async Task<ParkingLotEntity> UpdateAsync(ParkingLotEntity entity)
    {
        context.EnsureConnectionOpen();

        var sql = """
        UPDATE parking_lots 
        SET 
        display_name = @ParkingLotName, 
        prefecture_id = @Prefecture, 
        zip_code = @ZipCode, 
        street_address = @StreetAddress, 
        phone_number = @PhoneNumber, 
        fax_number = @FaxNumber, 
        max_capacity = @MaxCapacity,
        reservation_capacity = @ReservationCapacity,
        is_allows_boarding_alighting = @IsAllowsBoardingAlighting, 
        is_active = @IsActive 
        WHERE id = @Id 
        RETURNING * ; 
        """;

        var table = await context.Connection.QueryFirstAsync<ParkingLotTable>(
            sql, entity, context.Transaction
        );

        return table.ToDomain();
    }
    public async Task<ParkingLotEntity[]> GetAllAsync()
    {
        context.EnsureConnectionOpen();

        var sql = """
            SELECT * FROM parking_lots; 
        """;

        var tables = await context.Connection.QueryAsync<ParkingLotTable>(sql, transaction: context.Transaction);

        if(tables.FirstOrDefault() is null)
        {
            return [];
        }

        return tables.Select(t => t.ToDomain()).ToArray();
    }

    public async Task<bool> DeleteAsync(Guid parkingLotId)
    {

        context.EnsureConnectionOpen();

        var sql = """
        DELETE FROM parking_lots WHERE id = @Id
        """;

        var ret = await context.Connection.ExecuteAsync(sql, new { Id = parkingLotId }, context.Transaction);

        return ret == 1;
    }
}