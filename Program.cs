using Microsoft.EntityFrameworkCore;
using Ping_Project.Entities;
using Ping_Project.Infrastructure;
using Ping_Project.Infrastructure.Repository;
using Ping_Project.Services;
using Ping_Project.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Encrypt>();
builder.Services.AddSingleton<Decrypt>();
builder.Services.AddHostedService<TcpReceiverService>();
builder.Services.AddScoped<LogRepository>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

string? GeminiApiKey =  builder.Configuration.GetValue<string>("GeminiApi");

var app = builder.Build();

if(args.Contains("--print-logs"))
{
    using var scope = app.Services.CreateScope();
    var repo = scope.ServiceProvider.GetRequiredService<LogRepository>();
    var logs = await repo.PrintAllLogs();
    foreach (var log in logs)
    {
        Console.WriteLine(log);
    }

    return;
}


app.UseHttpsRedirection();



app.Run();
