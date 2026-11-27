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
using BusReservationService.Infra.Database;
using BusReservationService.Infra.Database.Service.Query;
using BusReservationService.Infra.Database.Service;
using Application.Core.Services.Interface.Administrator;
using BusReservationService;
using BusReservationService.Domain.Service;
using BusReservationService.Database.Interface;
using BusReservationService.Application.Core.Services.Interface;


var builder = WebApplication.CreateBuilder(args);

// Database Management Service
builder.Services.AddSingleton<IDatabaseService, NpgsqlDatabaseService>();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var factory =
        sp.GetRequiredService<IDatabaseService>();

    return factory.CreateConnection();
});
builder.Services.AddScoped(sp => new DbSession(sp.GetRequiredService<IDbConnection>()));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DbSession>());
builder.Services.AddScoped<IDbContext>(sp => sp.GetRequiredService<DbSession>());

// Domain Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IParkingLotRepository, ParkingLotRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();


// Session Management Service
builder.Services.AddServiceSessionStore(builder.Configuration);

// Domain Services
builder.Services.AddScoped<IReservationNumberGenerator, ReservationNumberGenerator>();

// Application Service
builder.Services.AddScoped<IOnboardUserProfileService, OnboardUserProfileService>();
builder.Services.AddScoped<IRegistrationAccountService, RegistrationAccountService>();
builder.Services.AddScoped<IAuthAccountService, AuthUserAccountService>();
builder.Services.AddScoped<IParkingLotAvailabilityListService, ParkingLotAvailabilityListService>();

// Application Query Service
builder.Services.AddScoped<IAccountUserQueryService, AccountUserQueryService>();

// Reservation Command Service
builder.Services.AddScoped<IReservationCommandService, ReservationCommandService>();
ParkingLotAvailabilityOptions parkingLotAvailabilityOptions = new ParkingLotAvailabilityOptions();
builder.Services.AddSingleton(parkingLotAvailabilityOptions);
builder.Services.AddScoped<IParkingLotCommandService, ParkingLotCommandService>();

// Administrator Reservation Command Service
builder.Services.AddScoped<IAdministratorReservationCommandService, AdministratorReservationCommandService>();

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