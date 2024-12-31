using Diary.Domain.Entity;
using Diary.Domain.ValueObjects.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diary.DAL.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasConversion(
                name => name.FullName,
                value => new Name(value))
            .IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(2000);
        builder.HasIndex(x => x.LastEditedAt).IsDescending();

        //to configure nested value objects it could be like that
        /*builder.OwnsOne(p => p.Price, priceBuilder =>
        {
            priceBuilder.Property(m => m.Currency).HasMaxLength(3);
        });*/
    }
}