using Diary.Domain.Entity;
using Diary.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diary.DAL.Configurations;

public class ReportConfiguration:IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        
        //
        builder.HasData(new List<Report>
        {
            new()
            {
                Id = 1,
                CreatedBy = 1,
                Name = "Report1",
                Description = "Report1Description",
                UserId = 1

            },
            new()
            {
                Id = 2,
                CreatedBy = 1,
                Name = "Report2",
                Description = "Report2Description",
                UserId = 1
            }
        });
        //
    }
}