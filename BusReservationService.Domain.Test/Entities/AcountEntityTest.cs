using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject;
using FluentAssertions;

namespace BusReservationService.Domain.Test.Entities;

public class AcountEntityTest
{
    [Fact]
    public void CreateUserRoleAccount_ShouldCreateAccountWithUserRoleAndJustCreatedStatus()
    {
        var email = "test@email.com";
        var passwordHash = new byte[] { 1, 2, 3 };
        var passwordSalt = new byte[] { 2, 3, 4 };

        var entity = AccountEntity.CreateUserRoleAccount(
            email, 
            passwordHash, 
            passwordSalt);

        entity.Email.Should().Be(email);
        entity.PasswordHash.Should().BeEquivalentTo(passwordHash);
        entity.PasswordSalt.Should().BeEquivalentTo(passwordSalt);
        entity.AccountStatus.Should().Be(AccountStatus.JustCreated);
        entity.AccountRole.Should().Be(AccountRole.User);
        entity.LastLoginAt.Should().BeNull();
        entity.ActivatedAt.Should().BeNull();
    }

    [Fact]
    public void Activate_ShouldSetAccountStatusToActiveAndSetActivatedAt()
    {
        var email = "test@email.com";
        var passwordHash = new byte[] { 1, 2, 3 };
        var passwordSalt = new byte[] { 2, 3, 4 };

        var entity = AccountEntity.CreateUserRoleAccount(
            email, 
            passwordHash, 
            passwordSalt);

        entity.Activate();

        entity.AccountStatus.Should().Be(AccountStatus.Active);
        entity.ActivatedAt.Should().NotBeNull();
    }

}