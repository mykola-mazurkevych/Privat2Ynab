#pragma warning disable CA1034 // Nested types should not be visible

using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application.Handlers;
using Privat2Ynab.Application.Interfaces.Handlers;

namespace Privat2Ynab.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication() =>
            services
                .AddTransient<IAccountHandler, AccountHandler>()
                .AddTransient<ICategoryRuleHandler, CategoryRuleHandler>()
                .AddTransient<IPayeeRuleHandler, PayeeRuleHandler>()
                .AddTransient<IPlanHandler, PlanHandler>();
    }
}