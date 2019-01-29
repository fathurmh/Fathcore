using System;
using Fathcore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.Tests.Fakes
{
    public class FakeEntityMapping : EntityTypeConfiguration<FakeEntity>
    {
        protected override void PostConfigure(EntityTypeBuilder<FakeEntity> builder)
        {
            builder.HasIndex(entity => new { entity.EntityCode, entity.DeletedTime })
                .HasName(FormatName(UniqueIndexPrefix, nameof(FakeEntity), nameof(FakeEntity.EntityCode), nameof(FakeEntity.DeletedTime)))
                .IsUnique();

            builder.HasIndex(entity => entity.EntityName)
                .HasName(FormatName(IndexPrefix, nameof(FakeEntity), nameof(FakeEntity.EntityName)));
        }

        public override void Configure(EntityTypeBuilder<FakeEntity> builder)
        {
            builder.ToTable(nameof(FakeEntity));

            builder.Property(entity => entity.EntityCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(entity => entity.EntityName)
                .IsRequired()
                .HasMaxLength(400);

            builder.HasMany(entity => entity.FakeChildEntities).WithOne(entity => entity.FakeEntity);

            base.Configure(builder);
        }
    }
}
