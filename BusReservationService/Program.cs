using BusReservationService.Database.Repository;
using BusReservationService.Database.Service;
using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using BusReservationService.Services;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FastEndpoints.Security;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Database Management Service
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<IDbConnection>(sp =>
{
    var factory =
        sp.GetRequiredService<DatabaseService>();

    return factory.CreateConnection();
});

// Domain Repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Session Management Service
builder.Services.AddSingleton<ISessionService<Guid, SessionValue>, InMemorySessionService>();

// Application Service
builder.Services.AddScoped<IAccountService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

// Application Query Service


builder.Services
   .AddAuthenticationCookie(validFor: TimeSpan.FromMinutes(3), options: options =>
   {
       options.Cookie.Name = "__Host-BusReservationService.SessionId";

   }) //configure cookie auth
   .AddAuthorization(options =>
   {
       options.AddPolicy("VerifiedUser", x => x.RequireRole("User").RequireClaim("SessionId"));
       options.AddPolicy("AdminOnly",x => x.RequireRole("Admin").RequireClaim("SessionId"));

   }) //add this
   .AddFastEndpoints();

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
