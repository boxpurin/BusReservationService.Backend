using BusReservationService.Endpoints;
using BusReservationService.Redis;
using BusReservationService.Database.Repository;
using BusReservationService.Database.Service;
using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using BusReservationService.Services;
using BusReservationService.Services.Interface;
using BusReservationService.Services.Query;
using FastEndpoints;
using FastEndpoints.Security;
using System.Data;


var builder = WebApplication.CreateBuilder(args);

// Database Management Service
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var factory =
        sp.GetRequiredService<IDatabaseService>();

    return factory.CreateConnection();
});

// Domain Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Session Management Service
builder.Services.AddServiceSessionStore(builder.Configuration);

// Application Service
builder.Services.AddScoped<IOnboardUserProfileService, OnboarduserProfileService>();
builder.Services.AddScoped<IRegistrationAccountService, RegistrationAccountService>();

// Application Query Service
builder.Services.AddScoped<IAccountUserQueryService, AccountUserQueryService>();

builder.Services.AddPresentationEndpoints(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var dbService = app.Services.GetRequiredService<DatabaseService>();
dbService.ConfigureMappings();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication()
   .UseAuthorization()
   .UseDefaultExceptionHandler()
   .UseFastEndpoints();

app.Run();

// テストプロジェクトから参照できるように公開する
public class ApiMarker { }