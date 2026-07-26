using Microsoft.EntityFrameworkCore;

using Privat2Ynab.Domain.Accounts;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Infrastructure.Persistence;

internal sealed class Privat2YnabDbContext(DbContextOptions<Privat2YnabDbContext> options) :
    DbContext(options)
{
    internal static string ConnectionString =>
        $"Data Source={Path.Combine(AppContext.BaseDirectory, "privat2ynab.db")}";

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<CategoryRule> CategoryRules => Set<CategoryRule>();
    public DbSet<PayeeRule> PayeeRules => Set<PayeeRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Privat2YnabDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}