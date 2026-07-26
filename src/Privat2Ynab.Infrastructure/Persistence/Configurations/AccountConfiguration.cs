using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Privat2Ynab.Domain.Accounts;

namespace Privat2Ynab.Infrastructure.Persistence.Configurations;

internal sealed class AccountConfiguration :
    IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable(nameof(Privat2YnabDbContext.Accounts));
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(a => a.BudgetId).IsRequired();
        builder.Property(a => a.BudgetName).IsRequired();
        builder.Property(a => a.AccountId).IsRequired();
        builder.Property(a => a.AccountName).IsRequired();
        builder.Property(a => a.FileName).IsRequired();
        builder.Property(a => a.IsActive).IsRequired();
    }
}