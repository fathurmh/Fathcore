using System;
using Fathcore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.Tests.Fakes
{
    public class FakeChildEntityMapping : EntityTypeConfiguration<FakeChildEntity>
    {
        protected override void PostConfigure(EntityTypeBuilder<FakeChildEntity> builder)
        {
            builder.HasIndex(entity => new { entity.ChildEntityCode, entity.DeletedTime })
                .HasName(FormatName(UniqueIndexPrefix, nameof(FakeChildEntity), nameof(FakeChildEntity.ChildEntityCode), nameof(FakeChildEntity.DeletedTime)))
                .IsUnique();

            builder.HasIndex(entity => entity.ChildEntityName)
                .HasName(FormatName(IndexPrefix, nameof(FakeChildEntity), nameof(FakeChildEntity.ChildEntityName)));
        }

        public override void Configure(EntityTypeBuilder<FakeChildEntity> builder)
        {
            builder.ToTable(nameof(FakeEntity));

            builder.Property(entity => entity.ChildEntityCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(entity => entity.ChildEntityName)
                .IsRequired()
                .HasMaxLength(400);

            builder.HasOne(entity => entity.FakeEntity).WithMany(entity => entity.FakeChildEntities);

            base.Configure(builder);
        }
    }
}
