using Fathcore.EntityFramework.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fathcore.Tests.Fakes
{
    internal class StringQueryTypeMapping : QueryTypeConfiguration<StringQueryType>
    {
    }

    internal class ClassroomMapping : EntityTypeConfiguration<Classroom, int>
    {
        public override void Configure(EntityTypeBuilder<Classroom> builder)
        {
            builder.ToTable(nameof(Classroom));

            builder.Property(entity => entity.Code)
                .IsRequired();

            builder.HasMany(entity => entity.Students)
                .WithOne(entity => entity.Classroom)
                .HasForeignKey(entity => entity.ClassroomId)
                .IsRequired();

            base.Configure(builder);
        }
    }

    internal class StudentMapping : EntityTypeConfiguration<Student, int>
    {
        public override void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable(nameof(Student));

            builder.Property(entity => entity.Name)
                .IsRequired();

            builder.HasOne(entity => entity.Classroom)
                .WithMany(entity => entity.Students)
                .HasForeignKey(entity => entity.ClassroomId)
                .IsRequired();

            builder.HasOne(entity => entity.Address)
                .WithOne(entity => entity.Student)
                .HasForeignKey<Address>(entity => entity.StudentId)
                .IsRequired();

            base.Configure(builder);
        }
    }

    internal class AddressMapping : EntityTypeConfiguration<Address, int>
    {
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable(nameof(Address));

            builder.Property(entity => entity.Street)
                .IsRequired();

            base.Configure(builder);
        }
    }
}
