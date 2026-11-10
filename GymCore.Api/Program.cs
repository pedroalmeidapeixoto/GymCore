using GymCore.Application.Common;
using GymCore.Application.Plans;
using GymCore.Application.Roles;
using GymCore.Application.Students;
using GymCore.Application.Users;
using GymCore.Infrastructure.Persistence;
using GymCore.Infrastructure.Persistence.Repositories;
using GymCore.Infrastructure.Persistence.Seed;
using GymCore.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' não encontrada. Configure-a nos User Secrets.");

builder.Services.AddOpenApi();

builder.Services.AddDbContext<GymCoreDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<GymCoreDbContext>());
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<PlanService>();
builder.Services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<StudentService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedReferenceDataAsync();

    if (app.Environment.IsDevelopment())
    {
        await seeder.SeedDevelopmentDataAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();