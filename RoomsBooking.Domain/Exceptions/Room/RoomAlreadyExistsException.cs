using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions.Room;

public class RoomAlreadyExistsException(string number) : ConflictException($"Комната с таким номером '{number}' уже существует.");