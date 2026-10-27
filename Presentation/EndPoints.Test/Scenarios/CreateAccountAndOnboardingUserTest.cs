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

        var (signOutRsp, _) = await App.Client.POSTAsync<Signout, SignOutRequest, SignOutResponse>(new SignOutRequest());
        signOutRsp.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
    }
}