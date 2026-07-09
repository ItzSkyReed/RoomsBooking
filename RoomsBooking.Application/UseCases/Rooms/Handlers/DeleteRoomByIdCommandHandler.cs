using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Commands;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

// TODO: Когда будет готово бронирование комнат, надо явно запрещать удаление если есть активные брони или делать мягкое удаление
public class DeleteRoomByIdCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<DeleteRoomByIdCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoomByIdCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Rooms.Where(r => r.Id == request.Id).ExecuteDeleteAsync(cancellationToken);
        return Unit.Value;
    }
}