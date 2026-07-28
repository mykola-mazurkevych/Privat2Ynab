using System.CommandLine;

using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application.Interfaces.Handlers;

namespace Privat2Ynab.Console.Commands;

internal static class RootCommandExtensions
{
    extension(RootCommand rootCommand)
    {
        public RootCommand Configure(IServiceProvider serviceProvider)
        {
            var statementsHandler = serviceProvider.GetRequiredService<IStatementsHandler>();
            rootCommand.SetAction((_, cancellationToken) => statementsHandler.HandleAsync(cancellationToken));

            return rootCommand
                .AddAccountCommands(serviceProvider)
                .AddCategoryRuleCommands(serviceProvider)
                .AddPayeeRuleCommands(serviceProvider)
                .AddPlanCommands(serviceProvider);
        }
    }
}