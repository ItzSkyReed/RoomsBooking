using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomsBooking.Domain.Entities;

namespace RoomsBooking.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();
        // Узнал, что есть https://postgrespro.ru/docs/postgresql/18/citext#CITEXT-RATIONALE, он удобнее в данном случае, тк
        // приходится переводить в нижний регистр каждый раз при сравнении.
        builder.Property(x => x.Email).HasColumnType("citext").IsRequired().HasMaxLength(255);
        builder.Property(x => x.PasswordHash).UseCollation("C").IsRequired().HasMaxLength(255);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

        builder.HasIndex(x => x.Email).IsUnique();
    }
}