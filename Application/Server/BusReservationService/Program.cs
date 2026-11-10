using BusReservationService.Endpoints;
using BusReservationService.Redis;
using BusReservationService.Database.Repository;
using BusReservationService.Database.Service;
using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using BusReservationService.Services;
using BusReservationService.Services.Interface;
using FastEndpoints;
using System.Data;
using BusReservationService.Infra.Database.Service.Query;
using BusReservationService.Infra.Database.Service;


var builder = WebApplication.CreateBuilder(args);

// Database Management Service
builder.Services.AddSingleton<IDatabaseService, NpgsqlDatabaseService>();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var factory =
        sp.GetRequiredService<IDatabaseService>();

    return factory.CreateConnection();
});

// Domain Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();


// Session Management Service
builder.Services.AddServiceSessionStore(builder.Configuration);

// Application Service
builder.Services.AddScoped<IOnboardUserProfileService, OnboardUserProfileService>();
builder.Services.AddScoped<IRegistrationAccountService, RegistrationAccountService>();
builder.Services.AddScoped<IAuthAccountService, AuthUserAccountService>();

// Application Query Service
builder.Services.AddScoped<IAccountUserQueryService, AccountUserQueryService>();

// Reservation Command Service
builder.Services.AddScoped<IReservationCommandService, ReservationCommandService>();

builder.Services.AddPresentationEndpoints(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

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