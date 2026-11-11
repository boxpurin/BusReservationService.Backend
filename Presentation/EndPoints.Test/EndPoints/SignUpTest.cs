using BusReservationService.Endpoints.Test;
using BusReservationService.Endpoints.Auth;
using FastEndpoints.Testing;
using FastEndpoints;
using Shouldly;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Net.Http.Headers;

namespace BusReservationService.Endpoints.Test.Endpoints;

[Priority(1)]
[Collection<CollectionEndPoints>]
public class SignUpTest(MyApp App) : TestBase
{
    [Fact]
    public async Task SingupRequest_Need_Email_And_Password()
    {
        await App.ResetDatabaseAsync();
        
        var req = new SignupRequest("valid@email.com", "passw0rd");
        var (rsp, res) = await App.Client.POSTAsync<Signup, SignupRequest, SignupResponse>(
            req
        );

        rsp.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task When_Signup_Successed_Returned_Response_Method_Created_With_AccountModel()
    {
        await App.ResetDatabaseAsync();

        var req = new SignupRequest("valid@email.com", "validpassw0rd");
        var (signUpMsg, res) = await App.Client.POSTAsync<Signup, SignupRequest, SignupResponse>(
            req
        );
        
        var options = App.Services
        .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
        .Get(CookieAuthenticationDefaults.AuthenticationScheme);


        signUpMsg.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);

        res.Account.Email.ShouldBe(req.Email);
        res.Account.AccountRole.ShouldBe(Domain.ValueObject.AccountRole.AccountRole.User);
        res.Account.AccountStatus.ShouldBe(Domain.ValueObject.AccountStatus.AccountStatus.OnBoarding);
        res.Account.ActivatedAt.ShouldBeNull();
    }

    [Fact]
    public async Task When_Send_Request_With_Invalied_Email_Then_Return_BadRequest()
    {
        await App.ResetDatabaseAsync();

        var req = new SignupRequest("invalidemail", "validpassw0rd");
        var (rsp, res) = await App.Client.POSTAsync<Signup, SignupRequest, SignupResponse>(
            req
        );

        rsp.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        res.ShouldBeNull();
    }
    
    [Fact]
    public async Task When_Send_Request_With_Too_Short_Password_Then_Return_BadRequest()
    {
        await App.ResetDatabaseAsync();

        var req = new SignupRequest("valid@email.com", "short");
        var (rsp, res) = await App.Client.POSTAsync<Signup, SignupRequest, SignupResponse>(
            req
        );

        rsp.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        res.ShouldBeNull();
    }

    [Fact]
    public async Task When_Email_Already_Used_Then_Return_Conflict()
    {
        await App.ResetDatabaseAsync();

        var req = new SignupRequest("valid@email.com", "validpassw0rd");

        await App.Client.POSTAsync<Signup, SignupRequest, SignupResponse>(
            req
        );

        var (rsp, res) = await App.Client.POSTAsync<Signup, SignupRequest, SignupResponse>(
            req
        );

        rsp.StatusCode.ShouldBe(System.Net.HttpStatusCode.Conflict);
    }
}