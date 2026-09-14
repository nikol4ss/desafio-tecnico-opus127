using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opus127.Dengue.Domain.Entities;

namespace Opus127.Dengue.Infrastructure.Persistence.Configurations;

public sealed class DengueRecordConfiguration : IEntityTypeConfiguration<DengueRecord>
{
    public void Configure(EntityTypeBuilder<DengueRecord> builder)
    {
        builder.ToTable("DengueRecords");
        builder.HasKey(record => record.Id);

        builder.Property(record => record.Id).UseIdentityColumn();
        builder.Property(record => record.Geocode).IsRequired();
        builder.Property(record => record.EpidemiologicalYear).IsRequired();
        builder.Property(record => record.EpidemiologicalWeek).IsRequired();
        builder.Property(record => record.WeekStartDate).HasColumnType("date").IsRequired();
        builder.Property(record => record.EstimatedCases).IsRequired();
        builder.Property(record => record.NotifiedCases).IsRequired();
        builder.Property(record => record.AlertLevel).IsRequired();
        builder.Property(record => record.ModelVersion).HasMaxLength(32);
        builder.Property(record => record.SyncedAtUtc).IsRequired();

        builder.HasIndex(record => new
        {
            record.Geocode,
            record.EpidemiologicalYear,
            record.EpidemiologicalWeek
        })
            .IsUnique()
            .HasDatabaseName("UX_DengueRecords_Geocode_Year_Week");

        builder.HasIndex(record => record.WeekStartDate)
            .HasDatabaseName("IX_DengueRecords_WeekStartDate");
    }
}
