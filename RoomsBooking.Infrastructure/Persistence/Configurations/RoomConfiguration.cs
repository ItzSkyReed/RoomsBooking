using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Number).IsRequired().HasColumnType("citext").HasMaxLength(100).HasComment("Номер кабинета");
        builder.Property(x => x.Capacity).IsRequired();
        builder.Property(x => x.Floor).IsRequired();
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(2000);

        builder.HasIndex(x => x.Number).IsUnique().HasDatabaseName("ix_rooms_number");
    }
}