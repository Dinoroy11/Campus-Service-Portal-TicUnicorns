using CampusServicePortal.Modules.Identity.Entities;
using CampusServicePortal_TicUnicorns.Modules.Students.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusServicePortal_TicUnicorns.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Student");

        builder.HasKey(x => x.StudentId);

        builder.Property(x => x.FirstName)
            .IsRequired();

        builder.Property(x => x.LastName)
            .IsRequired();

        builder.Property(x => x.DateOfBirth)
            .IsRequired(false);

        builder.Property(x => x.Gender)
            .IsRequired(false);

        builder.Property(x => x.Email)
            .IsRequired(false);

        builder.Property(x => x.PhoneNumber)
            .IsRequired(false);

        builder.Property(x => x.AdmissionDate)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.MasterStudentId)
            .IsUnique();

        builder.HasIndex(x => x.UserId)
            .IsUnique()
            .HasFilter("[UserId] IS NOT NULL");

        builder.HasOne<StudentMasterList>()
            .WithOne()
            .HasForeignKey<Student>(x => x.MasterStudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}