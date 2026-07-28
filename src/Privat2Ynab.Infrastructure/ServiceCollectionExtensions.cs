#pragma warning disable CA1034 // Nested types should not be visible

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Infrastructure.Persistence;
using Privat2Ynab.Infrastructure.Services;

namespace Privat2Ynab.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure() =>
            services
                .AddDbContext()
                .AddTransient<IRepository, Repository>()
                .AddTransient<IOutput, ConsoleOutput>()
                .AddTransient<IStatementsReader, StatementsReader>()
                .AddTransient<IYnabClient, YnabClient>();

        private IServiceCollection AddDbContext() =>
            services
                .AddDbContext<Privat2YnabDbContext>(builder => builder.UseSqlite(Privat2YnabDbContext.ConnectionString))
                .AddTransient(provider => provider.GetRequiredService<Privat2YnabDbContext>().GetService<IMigrator>());
    }
}