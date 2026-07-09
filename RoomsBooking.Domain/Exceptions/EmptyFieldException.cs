using RoomsBooking.Domain.Exceptions.Base;

namespace RoomsBooking.Domain.Exceptions;

public class EmptyFieldException(string field)
    : DomainBadRequestException($"Поле {field} не может быть пустым", field);