using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using RoomsBooking.Domain.Exceptions.Bookings;
using RoomsBooking.Domain.Exceptions.Room;
using RoomsBooking.Domain.Exceptions.User;

namespace RoomsBooking.Infrastructure.Persistence;

public sealed class ExceptionTranslationInterceptor : SaveChangesInterceptor
{
    private static readonly Dictionary<string, Func<Exception>> ExceptionMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["ix_users_email"] = () => new UserAlreadyExistsException(),
        ["ix_rooms_number"] = () => new RoomAlreadyExistsException(),
        ["EXCLUDE_overlapping_bookings"] = () => new RoomAlreadyBooked()
    };

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (eventData.Exception is DbUpdateException { InnerException: PostgresException pgEx })
            throw Translate(pgEx, eventData.Exception);

        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        return eventData.Exception is DbUpdateException { InnerException: PostgresException pgEx }
            ? throw Translate(pgEx, eventData.Exception)
            : base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private static Exception Translate(PostgresException pgEx, Exception fallbackException)
    {
        // Базовая проверка, что это именно те типы ошибок БД, которые мы вообще хотим перехватывать
        var isConstraintViolation = pgEx.SqlState is PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.ExclusionViolation;

        if (isConstraintViolation &&
            pgEx.ConstraintName != null &&
            ExceptionMap.TryGetValue(pgEx.ConstraintName, out var exceptionFactory))
        {
            return exceptionFactory();
        }

        return fallbackException;
    }
}