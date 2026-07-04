using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Domain.Exceptions.Room;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

// TODO: Когда будет готово бронирование комнат, надо явно запрещать удаление если есть активные брони или делать мягкое удаление
public class DeleteRoomByIdCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<DeleteRoomByIdCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoomByIdCommand request, CancellationToken cancellationToken)
    {
        var rows = await dbContext.Rooms.Where(r => r.Id == request.Id).ExecuteDeleteAsync(cancellationToken);
        return rows == 0
            ? throw new RoomNotFoundException(request.Id) // Если мы хотим идемпотентности, то независимо от результата возвращаем Unit
            : Unit.Value;
    }
}