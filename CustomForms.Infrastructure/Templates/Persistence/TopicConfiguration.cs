using CustomForms.Domain.Forms;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using CustomForms.Domain.Templates;

namespace CustomForms.Infrastructure.Templates.Persistence;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
	public void Configure(EntityTypeBuilder<Topic> builder)
	{
		builder.HasKey(t => t.Id);

		builder.Property(t => t.Name)
			.IsRequired()
			.HasMaxLength(100);

		builder.HasIndex(t => t.Name).IsUnique();

		builder.HasData(
			new Topic(Guid.Parse("E4B8C8A0-5A8A-4D8A-8F8A-0C1B1C1D1E1F"), "Education"),
			new Topic(Guid.Parse("A1B2C3D4-E5F6-7890-ABCD-FE0987654321"), "Quiz"),
			new Topic(Guid.Parse("DEADBEEF-CAFE-BABE-FACE-000000000000"), "Poll"),
			new Topic(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Questionnaire"),
			new Topic(Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), "Other")
		);
	}
}
