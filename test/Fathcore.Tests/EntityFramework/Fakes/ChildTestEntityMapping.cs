using Fathcore.EntityFramework.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.Tests.EntityFramework.Fakes
{
    internal class ChildTestEntityMapping : EntityTypeConfiguration<ChildTestEntity>
    {
        public override void Configure(EntityTypeBuilder<ChildTestEntity> builder)
        {
            builder.ToTable(nameof(ChildTestEntity));

            builder.Property(entity => entity.ChildTestField)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(entity => entity.TestEntity).WithMany(entity => entity.ChildTestEntities).OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}
