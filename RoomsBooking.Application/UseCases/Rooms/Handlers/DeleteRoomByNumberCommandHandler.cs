using MediatR;
using Microsoft.EntityFrameworkCore;
using RoomsBooking.Application.Interfaces;
using RoomsBooking.Application.UseCases.Rooms.Commands;

namespace RoomsBooking.Application.UseCases.Rooms.Handlers;

// TODO: Когда будет готово бронирование комнат, надо явно запрещать удаление если есть активные брони или делать мягкое удаление
public class DeleteRoomByNumberCommandHandler(
    IAppDbContext dbContext)
    : IRequestHandler<DeleteRoomByNumberCommand, Unit>
{
    public async Task<Unit> Handle(DeleteRoomByNumberCommand request, CancellationToken cancellationToken)
    {
        await dbContext.Rooms.Where(r => r.Number == request.Number.Trim()).ExecuteDeleteAsync(cancellationToken);

        return Unit.Value;
    }
}