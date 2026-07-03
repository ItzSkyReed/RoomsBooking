namespace RoomsBooking.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public Guid UserId { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }

    private RefreshToken()
    {
    } // Для EF Core

    public RefreshToken(Guid userId, string token, DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Токен не может быть пустым", nameof(token));

        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Срок действия токена должен быть в будущем", nameof(expiresAt));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId не может быть пустым Guid", nameof(userId));

        Id = Guid.CreateVersion7();
        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
    }
}