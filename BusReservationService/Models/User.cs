using BusReservationService.Database.Table;

namespace BusReservationService.Models;

public class User
{
    public string FirstName { get; set;}
    public string LastName { get; set; }

    public string Phone { get; set; }

    public AccountModel Account { get; set; }

    public User(UserTable userTable, AccountTable accountTable)
    {
        FirstName = userTable.FirstName;
        LastName = userTable.LastName;
        Account = new AccountModel(accountTable.ToEntity());
    }
}
