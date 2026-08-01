using Microsoft.EntityFrameworkCore;
using RailBooking.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Connection string comes from configuration only - never hardcoded here.
// Docker Compose supplies it via the ConnectionStrings__RailBookingDb
// environment variable (see docker-compose.yml / .env.example); for local
// `dotnet run` outside Docker, set it with:
//   dotnet user-secrets set "ConnectionStrings:RailBookingDb" "Host=localhost;..."
var connectionString = builder.Configuration.GetConnectionString("RailBookingDb")
    ?? throw new InvalidOperationException(
        "Connection string 'RailBookingDb' is not configured. Set the " +
        "ConnectionStrings__RailBookingDb environment variable, or for local " +
        "development run: dotnet user-secrets set \"ConnectionStrings:RailBookingDb\" \"...\"");

builder.Services.AddDbContext<RailBookingDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Apply pending migrations and seed the rolling trip-departure window on
// startup, so `docker compose up` (or `dotnet run`) is a genuine one-shot
// setup with no separate manual migration step.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RailBookingDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedUpcomingTripDeparturesAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
