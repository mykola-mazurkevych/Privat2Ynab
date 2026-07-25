#pragma warning disable CA1034 // Nested types should not be visible

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Infrastructure.Persistence;

namespace Privat2Ynab.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure() =>
            services
                .AddDbContext();

        private IServiceCollection AddDbContext() =>
            services
                .AddDbContext<Privat2YnabDbContext>(builder => builder.UseSqlite(Privat2YnabDbContext.ConnectionString));
    }
}