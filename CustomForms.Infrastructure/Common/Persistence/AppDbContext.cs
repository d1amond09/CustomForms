using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CustomForms.Domain.Users;
using CustomForms.Domain.Forms;
using CustomForms.Infrastructure.Users.Persistence;
using CustomForms.Infrastructure.Templates.Persistence;
using CustomForms.Domain.Templates;

namespace CustomForms.Infrastructure.Common.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
	public DbSet<Template> Templates { get; set; } = null!;
	public DbSet<Question> Questions { get; set; } = null!;
	public DbSet<Form> Forms { get; set; } = null!;
	public DbSet<Answer> Answers { get; set; } = null!;
	public DbSet<Comment> Comments { get; set; } = null!;
	public DbSet<Like> Likes { get; set; } = null!;
	public DbSet<Tag> Tags { get; set; } = null!;
	public DbSet<Topic> Topics { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new RoleConfiguration());
		modelBuilder.ApplyConfiguration(new UserConfiguration());
		modelBuilder.ApplyConfiguration(new TemplateConfiguration());
		modelBuilder.ApplyConfiguration(new TopicConfiguration());
	}

	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		UpdateAuditEntities();
		return await base.SaveChangesAsync(cancellationToken);
	}

	public override int SaveChanges()
	{
		UpdateAuditEntities();
		return base.SaveChanges();
	}

	private void UpdateAuditEntities()
	{
		var entries = ChangeTracker
			.Entries()
			.Where(e => e.Entity is Domain.Common.AuditableEntity &&
						(e.State == EntityState.Added || e.State == EntityState.Modified));

		foreach (var entityEntry in entries)
		{
			var entity = (Domain.Common.AuditableEntity)entityEntry.Entity;
			var now = DateTime.UtcNow;

			if (entityEntry.State == EntityState.Added)
			{
				
				if (entity.CreatedDate == default)
				{
					entity.GetType().GetProperty(nameof(Domain.Common.AuditableEntity.CreatedDate))?
					   .SetValue(entity, now, null);
				}

			}
			else if (entityEntry.State == EntityState.Modified)
			{
				var method = entity.GetType().GetMethod("SetModifiedDate", [typeof(DateTime?)]);
				if (method != null)
				{
					method.Invoke(entity, [now]);
				}
				else 
				{
					entity.GetType().GetProperty(nameof(Domain.Common.AuditableEntity.LastModifiedDate))?
					   .SetValue(entity, now, null);
				}
			}
		}
	}
}
