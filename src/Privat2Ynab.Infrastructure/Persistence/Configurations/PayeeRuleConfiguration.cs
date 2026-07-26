using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Infrastructure.Persistence.Configurations;

internal sealed class PayeeRuleConfiguration :
    IEntityTypeConfiguration<PayeeRule>
{
    public void Configure(EntityTypeBuilder<PayeeRule> builder)
    {
        builder.ToTable(nameof(Privat2YnabDbContext.PayeeRules));
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(p => p.PlanId).IsRequired();
        builder.Property(p => p.Memo).IsRequired();
        builder.Property(p => p.MatchType).IsRequired().HasConversion<string>();
        builder.Property(p => p.YnabId).IsRequired();
        builder.Property(p => p.Name).IsRequired();

        builder.HasOne(p => p.Plan).WithMany().HasForeignKey(p => p.PlanId);

        builder.Navigation(p => p.Plan).AutoInclude();
    }
}