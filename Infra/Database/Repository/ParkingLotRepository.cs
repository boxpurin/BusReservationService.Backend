using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.Prefecture;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using Dapper;
using System.Data;

namespace BusReservationService.Database.Repository;

public class ParkingLotRepository(IDbContext context) : IParkingLotRepository
{
    public async Task<ParkingLotEntity> AddAsync(AddParkingLotDto dto)
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
            dto.ParkingLotName,
            dto.ZipCode,
            dto.Prefecture,
            dto.StreetAddress,
            dto.PhoneNumber,
            dto.FaxNumber,
            dto.MaxCapacity,
            dto.ReservationCapacity,
            dto.IsAllowsBoardingAlighting,
            dto.IsActive
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

        var tables = await context.Connection.QueryAsync<ParkingLotTable>(sql);

        if(tables.FirstOrDefault() is null)
        {
            return [];
        }

        return tables.Select(t => t.ToDomain()).ToArray();
    }


    public async Task<ParkingLotEntity?> FindByParkingLotIdAsync(Guid parkingLotId)
    {
        context.EnsureConnectionOpen();

        var sql = """
            SELECT * FROM parking_lots
            WHERE id = @Id; 
        """;

        var table = await context.Connection.QueryFirstOrDefaultAsync<ParkingLotTable>(
            sql, new { Id = parkingLotId }
        );

        return table?.ToDomain();
    }

    public async Task<ParkingLotEntity?> FindByParkingLotIdForUpdateAsync(Guid parkingLotId)
    {
        context.EnsureConnectionOpen();

        var sql = """
            SELECT * FROM parking_lots
            WHERE id = @Id
            FOR UPDATE;
        """;

        var table = await context.Connection.QueryFirstOrDefaultAsync<ParkingLotTable>(
            sql, new { Id = parkingLotId }
        );

        return table?.ToDomain();
    }

    public async Task<ParkingLotEntity[]> FindByPrefactureIdAsync(Prefecture prefecture)
    {        
        context.EnsureConnectionOpen();

        var sql = """
            SELECT * FROM parking_lots 
            WHERE prefecture_id = @Id;
        """;

        var tables = await context.Connection.QueryAsync<ParkingLotTable>(sql, new { Id = prefecture });

        if(tables.FirstOrDefault() is null)
        {
            return [];
        }

        return tables.Select(t => t.ToDomain()).ToArray();
    }

    public async Task<bool> ExistByParkingLotIdAsync(Guid parkingLotId)
    {
        context.EnsureConnectionOpen();

        var sql = """
            SELECT COUNT(*) FROM parking_lots
            WHERE id = @Id; 
        """;

        var row = await context.Connection.ExecuteScalarAsync<int>(
            sql, new { Id = parkingLotId }
        );

        return row > 0;
    }
    public async Task<bool> ExistByPrefectureAsync(Prefecture prefecture)
    {
        context.EnsureConnectionOpen();

        var sql = """
            SELECT COUNT(*) FROM parking_lots
            WHERE prefecture_id = @Id; 
        """;

        var row = await context.Connection.ExecuteScalarAsync<int>(
            sql, new { Id = prefecture }
        );

        return row > 0;
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
