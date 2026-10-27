using GymCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using GymCore.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' não encontrada. Configure-a nos User Secrets.");

builder.Services.AddOpenApi();

builder.Services.AddDbContext<GymCoreDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedReferenceDataAsync();

   if (app.Environment.IsDevelopment())
{
    await seeder.SeedDevelopmentDataAsync();

}
   if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
}


app.Run();