using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Authentication.Commands;

namespace RoomsBooking.Application.UseCases.Authentication.Handlers;

public partial class DeleteExpiredTokensCommandHandler(
    IAppDbContext dbContext,
    ILogger<DeleteExpiredTokensCommandHandler> logger)
    : IRequestHandler<DeleteExpiredTokensCommand, Unit>
{
    public async Task<Unit> Handle(DeleteExpiredTokensCommand request, CancellationToken cancellationToken)
    {
        var deletedCount = await dbContext.RefreshTokens
            .Where(x => x.ExpiresAt <= DateTimeOffset.UtcNow)
            .ExecuteDeleteAsync(cancellationToken);

        if (deletedCount > 0)
            LogExpiredRefreshTokensDeletion(deletedCount);

        return Unit.Value;
    }

    [LoggerMessage(LogLevel.Information, "Успешно удалено {Count} протухших refresh-токенов")]
    partial void LogExpiredRefreshTokensDeletion(int count);
}