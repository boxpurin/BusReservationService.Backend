using BusReservationService.Domain.ValueObject;

namespace BusReservationService.Domain.Entity;

/// <summary>
/// 人そのもの
/// </summary>
public class UserEntity
{
    public Guid AccountId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string ZipCode { get; private set; } 
    public string StreetAddress { get; private set; }
    public string PhoneNumber { get; private set; }
    public string FaxNumber { get; private set; }
    public UserType UserType { get; private set; }

    private UserEntity(
        Guid accountId , 
        string firstName, 
        string lastName,  
        string zipCode, 
        string streetAddress,
        string phoneNumber, 
        string faxNumber, 
        UserType userType)
    {
        AccountId = accountId;
        FirstName = firstName;
        LastName = lastName;
        ZipCode = zipCode;
        StreetAddress = streetAddress;
        PhoneNumber = phoneNumber;
        FaxNumber = faxNumber;
        UserType = userType;
    }

    public static UserEntity CreateUserEntity(
        AccountEntity accountEntity , 
        string firstName, 
        string lastName,  
        string zipCode, 
        string streetAddress,
        string phoneNumber, 
        string faxNumber, 
        UserType userType)
    {
        return new UserEntity(
            accountEntity.Id,
            firstName, 
            lastName,  
            zipCode, 
            streetAddress,  
            phoneNumber, 
            faxNumber, 
            userType
        );
    }

    
    public static UserEntity CreateUserEntity(
        Guid accountId, 
        string firstName, 
        string lastName,  
        string zipCode, 
        string streetAddress,
        string phoneNumber, 
        string faxNumber, 
        UserType userType)
    {
        return new UserEntity(
            accountId,
            firstName, 
            lastName,  
            zipCode, 
            streetAddress,  
            phoneNumber, 
            faxNumber, 
            userType
        );
    }
}
