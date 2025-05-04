using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CustomForms.Domain.Users;

namespace CustomForms.Infrastructure.Users.Persistence;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.Property(u => u.FirstName).HasMaxLength(100);
		builder.Property(u => u.LastName).HasMaxLength(100);

		builder.Property(u => u.RefreshToken).HasMaxLength(256); 
		builder.HasIndex(u => u.RefreshToken).IsUnique();

		builder.Property(u => u.IsBlocked).IsRequired().HasDefaultValue(false);
	}
}
