using E_Commerce.Application.Mapper;
using E_Commerce.DependencyInjections;
using E_Commerce.Domain.Entities;
using E_Commerce.MiddleWare;
using ECommerce.Infrastructure.Context;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);


var solutionDir = Directory.GetParent(builder.Environment.ContentRootPath)!.FullName;
var logPath = Path.Combine(solutionDir, "logs", "log-.log");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: logPath,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        shared: true
    )
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("=== ECommerce API Starting ===");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    return ConnectionMultiplexer.Connect(
        configuration.GetConnectionString("Redis") ?? "");
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();
builder.Services.AddProjectServices();
builder.Services.AddIdentity(builder.Configuration);
builder.Services.AddSwaggerIdentity();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ECommerceConnection")));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

    var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    db.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var roles = new[] { "Admin", "User", "Seller", "Delivery" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminUserName = "admin";
    var adminUser = await userManager.FindByNameAsync(adminUserName);
    if(adminUser == null)
    {
        adminUser = new ApplicationUser { UserName = adminUserName };
        await userManager.CreateAsync(adminUser, configuration["Admin:Password"]);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

   // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    //dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
