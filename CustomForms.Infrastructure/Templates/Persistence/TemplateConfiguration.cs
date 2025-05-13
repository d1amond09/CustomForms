using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CustomForms.Domain.Forms;
using CustomForms.Domain.Templates;

namespace CustomForms.Infrastructure.Templates.Persistence;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
	public void Configure(EntityTypeBuilder<Template> builder)
	{
		builder.HasKey(t => t.Id);

		builder.Property(t => t.Title)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(t => t.Description)
			.HasMaxLength(4000);

		builder.Property(t => t.ImageUrl)
			.HasMaxLength(1024);

		builder.Property(t => t.AuthorId).IsRequired();
		builder.Property(t => t.TopicId).IsRequired();

		builder.HasIndex(t => t.AuthorId);
		builder.HasIndex(t => t.TopicId);
		builder.HasIndex(t => t.CreatedDate);

		builder.HasOne(t => t.Author)
			.WithMany(u => u.CreatedTemplates)
			.HasForeignKey(t => t.AuthorId)
			.OnDelete(DeleteBehavior.Restrict); 

		builder.HasOne(t => t.Topic)
			.WithMany(to => to.Templates)
			.HasForeignKey(t => t.TopicId)
			.OnDelete(DeleteBehavior.Restrict); 

		builder.HasMany(t => t.Questions)
			.WithOne(q => q.Template)
			.HasForeignKey(q => q.TemplateId)
			.OnDelete(DeleteBehavior.Cascade); 

		builder.HasMany(t => t.Forms)
			.WithOne(f => f.Template)
			.HasForeignKey(f => f.TemplateId)
			.OnDelete(DeleteBehavior.Restrict); 

		builder.HasMany(t => t.Comments)
			.WithOne(c => c.Template)
			.HasForeignKey(c => c.TemplateId)
			.OnDelete(DeleteBehavior.Cascade); 

		builder.HasMany(t => t.Likes)
			.WithOne(l => l.Template)
			.HasForeignKey(l => l.TemplateId)
			.OnDelete(DeleteBehavior.Cascade); 

		builder.HasMany(t => t.Tags)
			.WithMany(tg => tg.Templates)
			.UsingEntity(j => j.ToTable("TemplateTags")); 

		builder.HasMany(t => t.AllowedUsers)
			.WithMany(u => u.AllowedTemplates)
			.UsingEntity(j => j.ToTable("TemplateAllowedUsers")); 
	}
}
