using BusReservationService.Domain.Entity;
using BusReservationService.Dto.Request;
using BusReservationService.Share;

namespace BusReservationService.Services.Interface;


internal interface IAccountService{
    Task<AccountEntity?> RegistorationAccount(RegistrationAccount dto);
    Task<AccountEntity?> SinginAccount(string email, string password);
    Task<AccountEntity?> SuspendAccountAsync(Guid accountId);
}

