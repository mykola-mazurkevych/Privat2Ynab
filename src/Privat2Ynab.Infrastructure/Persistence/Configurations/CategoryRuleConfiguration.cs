using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Infrastructure.Persistence.Configurations;

internal sealed class CategoryRuleConfiguration :
    IEntityTypeConfiguration<CategoryRule>
{
    public void Configure(EntityTypeBuilder<CategoryRule> builder)
    {
        builder.ToTable(nameof(Privat2YnabDbContext.CategoryRules));
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(c => c.Memo).IsRequired();
        builder.Property(c => c.Type).IsRequired().HasConversion<string>();
        builder.Property(c => c.CategoryGroupName).IsRequired();
        builder.Property(c => c.CategoryName).IsRequired();

        builder.HasIndex(c => c.Memo).IsUnique();
    }
}