using Diary.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diary.DAL.Configurations;

public class ReportConfiguration:IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.Property(report => report.Id).ValueGeneratedOnAdd();
        builder.Property(report => report.Name).IsRequired().HasMaxLength(100);
        builder.Property(report => report.Description).IsRequired().HasMaxLength(2000);
    }
}