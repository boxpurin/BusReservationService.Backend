using BusReservationService.Endpoints.Auth;
using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Renci.SshNet.Sftp;
using Shouldly;

namespace BusReservationService.Endpoints.Test.Scenarios;

/// <summary>
/// Scenarioテスト
/// アカウント作成からユーザー情報の入力を実行し、
/// ログインを行う
/// </summary>
/// <param name="App"></param>
[Priority(2)]
[Collection<CollectionEndPoints>]
public class CreateAccountAndOnboardingUserTest(MyApp App) : TestBase
{
    [Fact]
    public async Task Create_Account_And_Onboarding_User()
    {
        await App.ResetDatabaseAsync();
        var options = App.Services
        .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
        .Get(CookieAuthenticationDefaults.AuthenticationScheme);

        var signUpRequest = new SignupRequest("validtest@email.com", "passw0rd");
        var (signUpMsg, signUpResponse) = await App.Client.POSTAsync<Signup, SignupRequest, SignupResponse>(
            signUpRequest
        );
        
        var activateReq = new ActivateRequest(
            "FisrtName",
            "LastName",
            "1234567",
            "東京都千代田区1-1-1",
            "0668785723",
            null,
            Domain.ValueObject.UserType.UserType.Personal
        );

        var (activateMsg, activateResponse) = await App.Client.POSTAsync<Activate, ActivateRequest, ActivateResponse>(
            activateReq
        );

        activateMsg.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);

        var signInReq = new SignInRequest(
            signUpRequest.Email, signUpRequest.Password
        );

        var (signInMsg, SignInRes) = await App.Client.POSTAsync<Signin, SignInRequest, SignInResponse>(
            signInReq
        );
        signInMsg.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

        // 1. このアプリのCookieが発行されていること
        signInMsg.Headers.Contains("Set-Cookie").ShouldBeTrue();

        var cookies = SetCookieHeaderValue.ParseList(
        signInMsg.Headers.GetValues("Set-Cookie").ToList());
    
        var cookie = cookies.Single(x =>
        x.Name.ToString() == options.Cookie.Name);
        
        var referenceTicket = options.TicketDataFormat
            .Unprotect(cookie.Value.ToString());

        referenceTicket.ShouldNotBeNull();

        // ASP.NET CoreがSessionStore参照用に設定する内部Claim
        var sessionKey = referenceTicket.Principal.FindFirst(
            "Microsoft.AspNetCore.Authentication.Cookies-SessionId")?.Value;

        sessionKey.ShouldNotBeNullOrWhiteSpace();
        options.SessionStore.ShouldNotBeNull();

        var ticket = await options.SessionStore.RetrieveAsync(sessionKey!, TestContext.Current.CancellationToken);

        ticket.ShouldNotBeNull();
        ticket.Principal.Identity!.IsAuthenticated.ShouldBeTrue();
        ticket.Principal.FindFirst("AccountId")?.Value
        .ShouldBe(activateResponse.Account.Id.ToString());

        ticket.Properties.ExpiresUtc.ShouldNotBeNull();
        (ticket.Properties.ExpiresUtc.Value > DateTimeOffset.UtcNow)
        .ShouldBeTrue();

        var (signOutMsg, _) = await App.Client.POSTAsync<Signout, EmptyRequest>();
        signOutMsg.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
    }
}