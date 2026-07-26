using System.CommandLine;

using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Interfaces.Handlers;

namespace Privat2Ynab.Console.Commands;

internal static class PlanCommands
{
    extension(RootCommand rootCommand)
    {
        public RootCommand AddPlanCommands(IServiceProvider serviceProvider)
        {
            var planHandler = serviceProvider.GetRequiredService<IPlanHandler>();

            rootCommand.Add(
                new Command("plans")
                {
                    CreateListPlansCommand(planHandler),
                    CreateAddPlanCommand(planHandler),
                    CreateDeletePlanCommand(planHandler),
                });

            return rootCommand;
        }
    }

    private static Command CreateListPlansCommand(IPlanHandler planHandler)
    {
        var listPlansCommand = new Command("list", "List Plans");
        listPlansCommand.SetAction((_, cancellationToken) => planHandler.ListAsync(cancellationToken));

        return listPlansCommand;
    }

    private static Command CreateAddPlanCommand(IPlanHandler planHandler)
    {
        var ynabIdOption = new Option<Guid>("--ynab-id")
        {
            Description = "YNAB Plan Id",
            Required = true,
        };
        var tokenOption = new Option<string>("--token")
        {
            Description = "YNAB Personal Access Token",
            Required = true,
        };

        var addPlanCommand = new Command("add", "Add new plan")
        {
            ynabIdOption,
            tokenOption,
        };

        addPlanCommand.SetAction((parseResult, cancellationToken) =>
            planHandler.AddAsync(
                new CreatePlanDto(
                    parseResult.GetRequiredValue(ynabIdOption),
                    parseResult.GetRequiredValue(tokenOption)),
                cancellationToken));

        return addPlanCommand;
    }

    private static Command CreateDeletePlanCommand(IPlanHandler planHandler)
    {
        var idOption = new Option<int>("--id")
        {
            Description = "Plan Id",
            Required = true,
        };

        var deletePlanCommand = new Command("delete", "Delete existing plan")
        {
            idOption,
        };

        deletePlanCommand.SetAction((parseResult, cancellationToken) =>
            planHandler.DeleteAsync(
                parseResult.GetRequiredValue(idOption),
                cancellationToken));

        return deletePlanCommand;
    }
}