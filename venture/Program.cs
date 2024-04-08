using Microsoft.EntityFrameworkCore;
using stock.Data;
using System.Text.Json.Serialization;
using stock.Model.Entity;
using stock.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Logging.AddConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "hh:mm:ss ";
});

builder.Services.AddScoped<StockService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
            .WithOrigins("https://localhost:44476", "http://localhost:44476") 
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); 
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    
    // Seed data logic remains the same
    if (!context.UserProfiles.Any())
    {
        var newUser = new UserProfile
        {
            Cash = 10000,
            CurrentDate = DateTime.Today,
            NetWorth = 10000
        };
        var netWorthHistory = new UserHistory
        {
            UserId = 1,
            Date = DateTime.Today,
            NetWorth = 10000
        };
        
        context.UserProfiles.Add(newUser);
        context.UserHistories.Add(netWorthHistory);
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
