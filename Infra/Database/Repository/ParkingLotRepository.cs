using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.Prefecture;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using Dapper;
using System.Data;

namespace BusReservationService.Database.Repository;

public class ParkingLotRepository(IDbConnection connection) : IConnection, IParkingLotRepository
{
    public async Task<ParkingLotEntity> AddAsync(AddParkingLotDto dto)
    {
        EnsureConnectionOpen(connection);
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

        var tx = connection.BeginTransaction();
        var table = await connection.QueryFirstAsync<ParkingLotTable>(
            sql, entity, tx
        );

        tx.Commit();

        return table.ToDomain();
    }
    
    public async Task<ParkingLotEntity> UpdateAsync(ParkingLotEntity entity)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        UPDATE parking_lots 
        SET 
        display_name = @ParkingLotName, 
        prefecture_id = @Prefecture, 
        zip_code = @ZipCode, 
        street_address = @StreetAddress, 
        max_capacity = @MaxCapacity,
        reservation_capacity = @ReservationCapacity,
        is_allows_boarding_alighting = @IsAllowsBoardingAlighting, 
        is_active = @IsActive 
        WHERE id = @Id 
        RETURNING * ; 
        """;

        var tx = connection.BeginTransaction();
        var table = await connection.QueryFirstAsync<ParkingLotTable>(
            sql, entity, tx
        );

        tx.Commit();

        return table.ToDomain();
    }
    public async Task<ParkingLotEntity[]> GetAllAsync()
    {
        EnsureConnectionOpen(connection);

        var sql = """
            SELECT * FROM parking_lots; 
        """;

        var tables = await connection.QueryAsync<ParkingLotTable>(sql);

        if(tables.FirstOrDefault() is null)
        {
            return [];
        }

        return tables.Select(t => t.ToDomain()).ToArray();
    }


    public async Task<ParkingLotEntity?> FindByParkingLotIdAsync(Guid parkingLotId)
    {        
        EnsureConnectionOpen(connection);

        var sql = """
            SELECT * FROM parking_lots
            WHERE id = @Id; 
        """;

        var table = await connection.QueryFirstOrDefaultAsync<ParkingLotTable>(
            sql, new { Id = parkingLotId }
        );

        return table?.ToDomain();
    }

    public async Task<ParkingLotEntity[]> FindByPrefactureIdAsync(Prefecture prefecture)
    {        
        EnsureConnectionOpen(connection);

        var sql = """
            SELECT * FROM parking_lots 
            WHERE prefecture_id = @Id;
        """;

        var tables = await connection.QueryAsync<ParkingLotTable>(sql, new { Id = prefecture });

        if(tables.FirstOrDefault() is null)
        {
            return [];
        }

        return tables.Select(t => t.ToDomain()).ToArray();
    }

    public async Task<bool> ExistByParkingLotIdAsync(Guid parkingLotId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
            SELECT COUNT(*) FROM parking_lots
            WHERE id = @Id; 
        """;

        var row = await connection.ExecuteAsync(
            sql, new { Id = parkingLotId }
        );

        return row > 0;
    }
    public async Task<bool> ExistByPrefectureAsync(Prefecture prefecture)
    {
        EnsureConnectionOpen(connection);

        var sql = """
            SELECT COUNT(*) FROM parking_lots
            WHERE prefecture_id = @Id; 
        """;

        var row = await connection.ExecuteAsync(
            sql, new { Id = prefecture }
        );

        return row > 0;
    }

    public async Task<bool> DeleteAsync(Guid parkingLotId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        DELETE FROM parking_lots WHERE parking_lot_id = @Id
        """;

        var tx = connection.BeginTransaction();

        var ret = await connection.ExecuteAsync(sql, new { id = parkingLotId }, tx);

        tx.Commit();

        return ret == 1;
    }

}
