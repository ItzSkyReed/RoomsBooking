using RoomsBooking.Domain.Interfaces;

namespace RoomsBooking.Domain.Entities;

public sealed class RefreshToken
{
    private RefreshToken()
    {
    } // Для EF Core

    public RefreshToken(Guid userId, string plainToken, DateTimeOffset expiresAt, ITokenHasher tokenHasher)
    {
        if (string.IsNullOrWhiteSpace(plainToken))
            throw new ArgumentException("Токен не может быть пустым", nameof(plainToken));

        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Срок действия токена должен быть в будущем", nameof(expiresAt));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId не может быть пустым Guid", nameof(userId));

        Id = Guid.CreateVersion7();
        UserId = userId;
        ExpiresAt = expiresAt;

        Token = tokenHasher.Hash(plainToken);
    }

    public Guid Id { get; private set; }

    // Теперь здесь хранится SHA256 хэш, а не голый токен
    public string Token { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    public bool IsValid(string plainToken, ITokenHasher tokenHasher)
    {
        return DateTimeOffset.UtcNow < ExpiresAt && tokenHasher.Verify(plainToken, Token);
    }
}