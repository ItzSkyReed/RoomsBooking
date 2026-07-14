using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using RoomsBooking.API;
using Testcontainers.PostgreSql;

namespace RoomsBooking.Tests.Setup;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder("postgres:18-alpine")
        .WithDatabase("test_db")
        .WithUsername("postgres")
        .WithPassword("test_password")
        .Build();

    public string ConnectionString { get; private set; } = string.Empty;

    public async Task InitializeAsync()
    {
        // Поднимаем контейнер перед запуском тестов
        await _dbContainer.StartAsync();
        ConnectionString = _dbContainer.GetConnectionString();
    }

    public new async Task DisposeAsync()
    {
        // Удаляем контейнер после завершения всех тестов
        await _dbContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            // Подменяем строку подключения на сгенерированную Testcontainers
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = ConnectionString
            });
        });
    }
}