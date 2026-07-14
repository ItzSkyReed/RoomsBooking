using MediatR;
using RoomsBooking.Application.UseCases.Authentication.Commands;

namespace RoomsBooking.API.HostedServices;

public partial class ExpiredTokensCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpiredTokensCleanupService> logger) : BackgroundService
{
    // Время запуска по UTC
    private const int TargetHourUtc = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogCleanupServiceStarted();

        // Это решает проблему, что если сервер долго лежат, то уже накопились протухшие токены
        await CleanupTokensAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;
            var nextRun = now.Date.AddHours(TargetHourUtc);

            // Если время прошло, то планируем на завтра
            if (now > nextRun)
                nextRun = nextRun.AddDays(1);

            var delay = nextRun - now;

            LogCleanupServiceDelayed(delay);

            try
            {
                await Task.Delay(delay, stoppingToken);
                await CleanupTokensAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // Приложение останавливается
                break;
            }
        }
    }

    private async Task CleanupTokensAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            await sender.Send(new DeleteExpiredTokensCommand(), stoppingToken);
        }
        catch (Exception ex)
        {
            // Если не перехватить исключение класс навсегда упадет
            LogExpiredRefreshTokensDeletionError(ex);
        }
    }

    [LoggerMessage(LogLevel.Error, "Произошла ошибка при удалении протухших токенов.")]
    partial void LogExpiredRefreshTokensDeletionError(Exception exception);

    [LoggerMessage(LogLevel.Information, "Сервис удаления протухших токенов запущен.")]
    partial void LogCleanupServiceStarted();

    [LoggerMessage(LogLevel.Information, "Следующая очистка токенов запланирована через {delay}")]
    partial void LogCleanupServiceDelayed(TimeSpan delay);
}