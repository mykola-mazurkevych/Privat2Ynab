using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Privat2Ynab.Domain.Plans;

namespace Privat2Ynab.Infrastructure.Persistence.Configurations;

internal sealed class PlanConfiguration :
    IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable(nameof(Privat2YnabDbContext.Plans));
        builder.HasKey(p => p.Id);

        builder.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(c => c.YnabId).IsRequired();
        builder.Property(c => c.Name).IsRequired().HasConversion<string>();
        builder.Property(c => c.Token).IsRequired();
    }
}