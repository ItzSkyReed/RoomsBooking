using System.ComponentModel.DataAnnotations;

namespace RoomsBooking.Domain.Entities;

public sealed class User
{
    private static readonly EmailAddressAttribute EmailAttribute = new();

    public Guid Id { get; private set; }

    /// <summary>
    /// Используется как login для входа, уникальный
    /// </summary>
    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public string Name { get; private set; } = null!;

    private User()
    {
    } // Для EF Core

    public User(string email, string passwordHash, string name)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailAttribute.IsValid(email))
            throw new ArgumentException("Неверный формат Email", nameof(email));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Имя не может быть пустым", nameof(name));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Хэш пароля пустой", nameof(passwordHash));

        Id = Guid.CreateVersion7();
        Email = email.ToLowerInvariant();
        PasswordHash = passwordHash;
        CreatedAt = DateTimeOffset.UtcNow;

        Name = name;
    }
}