using System.Data.Common;
using Npgsql;
using Respawn;

namespace RoomsBooking.Tests.Setup;

// Базовый класс, от которого будут наследоваться все тестовые классы
public abstract class BaseIntegrationTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly DbConnection _dbConnection = new NpgsqlConnection(factory.ConnectionString);
    protected readonly HttpClient Client = factory.CreateClient();
    private Respawner _respawner = null!;


    public async Task InitializeAsync()
    {
        await _dbConnection.OpenAsync();

        // создаем базу, игнорируем __EFMigrationsHistory
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });

        // сброс базы перед тестом
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task DisposeAsync()
    {
        await _dbConnection.DisposeAsync();
    }
}