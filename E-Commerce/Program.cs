using E_Commerce.DependencyInjections;
using ECommerce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

// Add services to the container.

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
