using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.User;

public class UserAlreadyExistsException(string email) : ConflictException($"Пользователь с Email '{email}' уже существует.");