using Fathcore.EntityFramework.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.Tests.EntityFramework.Fakes
{
    internal class TestEntityMapping : EntityTypeConfiguration<TestEntity>
    {
        public override void Configure(EntityTypeBuilder<TestEntity> builder)
        {
            builder.ToTable(nameof(TestEntity));

            builder.Property(entity => entity.TestField)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(entity => entity.ChildTestEntities).WithOne(entity => entity.TestEntity).OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
