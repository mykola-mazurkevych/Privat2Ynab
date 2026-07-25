// ReSharper disable UnusedMember.Global

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Privat2Ynab.Infrastructure.Persistence;

internal sealed class DesignTimeDbContextFactory :
    IDesignTimeDbContextFactory<Privat2YnabDbContext>
{
    public Privat2YnabDbContext CreateDbContext(string[] args)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<Privat2YnabDbContext>();

        dbContextOptionsBuilder.UseSqlite(Privat2YnabDbContext.ConnectionString);

        return new Privat2YnabDbContext(dbContextOptionsBuilder.Options);
    }
}