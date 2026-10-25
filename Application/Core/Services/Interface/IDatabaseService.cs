using System.Data;

namespace BusReservationService.Services.Interface;

public interface IDatabaseService
{
    IDbConnection CreateConnection();
    void ConfigureMappings();
}