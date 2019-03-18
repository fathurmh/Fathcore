using Fathcore.EntityFramework.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.EntityFramework.Fakes
{
    internal class StringQueryTypeMapping : QueryTypeConfiguration<StringQueryType>
    {
    }

    internal class AuthorEntityMapping : EntityTypeConfiguration<AuthorEntity>
    {
        public override void Configure(EntityTypeBuilder<AuthorEntity> builder)
        {
            builder.ToTable(nameof(AuthorEntity));

            builder.Property(entity => entity.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(entity => entity.Books).WithOne(entity => entity.Writer);

            base.Configure(builder);
        }
    }

    internal class BookEntityMapping : EntityTypeConfiguration<BookEntity>
    {
        public override void Configure(EntityTypeBuilder<BookEntity> builder)
        {
            builder.ToTable(nameof(BookEntity));

            builder.Property(entity => entity.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(entity => entity.Writer).WithMany(entity => entity.Books);
            builder.HasOne(entity => entity.Title).WithOne(entity => entity.Book).HasForeignKey<TitleEntity>(entity => entity.Id);

            base.Configure(builder);
        }
    }

    internal class TitleEntityMapping : EntityTypeConfiguration<TitleEntity>
    {
        public override void Configure(EntityTypeBuilder<TitleEntity> builder)
        {
            builder.ToTable(nameof(TitleEntity));

            builder.Property(entity => entity.Subject)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasOne(entity => entity.Book).WithOne(entity => entity.Title);

            base.Configure(builder);
        }
    }
}
