using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

try
{
    await DBInitializer.InitDb(app);
    Console.WriteLine("✅ Database initialized successfully");
}
catch (Exception e)
{
    Console.WriteLine($"❌ Database initialization failed:");
    Console.WriteLine($"Message: {e.Message}");
    Console.WriteLine($"Stack Trace: {e.StackTrace}");
    throw; // 重新抛出，让应用启动失败
}

app.Run();
