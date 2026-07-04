using Riok.Mapperly.Abstractions;
using RoomsBooking.Application.UseCases.Rooms.Commands;
using RoomsBooking.Application.UseCases.Rooms.Dtos;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Application.UseCases.Rooms.Mappers;

[Mapper]
public static partial class RoomMapper
{
    public static partial Room ToRoomEntity(this CreateRoomCommand command);

    public static partial RoomDto ToRoomDto(this Room entity);
}