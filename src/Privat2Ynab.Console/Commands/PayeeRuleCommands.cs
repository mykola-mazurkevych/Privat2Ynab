using System.CommandLine;

using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Console.Commands;

internal static class PayeeRuleCommands
{
    extension(RootCommand rootCommand)
    {
        public RootCommand AddPayeeRuleCommands(IServiceProvider serviceProvider)
        {
            var payeeRuleHandler = serviceProvider.GetRequiredService<IPayeeRuleHandler>();

            rootCommand.Add(
                new Command("payee-rules")
                {
                    CreateListPayeeRulesCommand(payeeRuleHandler),
                    CreateAddPayeeRuleCommand(payeeRuleHandler),
                    ////CreateUpdatePayeeRuleCommand(), // TODO: Decide if needed
                    CreateDeletePayeeRuleCommand(payeeRuleHandler),
                });

            return rootCommand;
        }
    }

    private static Command CreateListPayeeRulesCommand(IPayeeRuleHandler payeeRuleHandler)
    {
        var listPayeeRulesCommand = new Command("list", "List payee rules");
        listPayeeRulesCommand.SetAction((_, cancellationToken) => payeeRuleHandler.ListAsync(cancellationToken));

        return listPayeeRulesCommand;
    }

    private static Command CreateAddPayeeRuleCommand(IPayeeRuleHandler payeeRuleHandler)
    {
        var planIdOption = new Option<int>("--plan-id")
        {
            Description = "Plan Id",
            Required = true,
        };
        var memoOption = new Option<string>("--memo")
        {
            Description = "YNAB Transaction Memo",
            Required = true,
        };
        var matchTypeOption = new Option<StringMatchType>("--match-type")
        {
            Description = "String Match Type",
            Required = true,
        };
        var payeeNameOption = new Option<string>("--payee-name")
        {
            Description = "YNAB Payee Id",
            Required = true,
        };

        var addPayeeRuleCommand = new Command("add", "Add new Payee rule")
        {
            planIdOption,
            memoOption,
            matchTypeOption,
            payeeNameOption,
        };

        addPayeeRuleCommand.SetAction((parseResult, cancellationToken) =>
            payeeRuleHandler.AddAsync(
                new CreatePayeeRuleDto(
                    parseResult.GetRequiredValue(planIdOption),
                    parseResult.GetRequiredValue(memoOption),
                    parseResult.GetRequiredValue(matchTypeOption),
                    parseResult.GetRequiredValue(payeeNameOption)),
                cancellationToken));

        return addPayeeRuleCommand;
    }

    private static Command CreateDeletePayeeRuleCommand(IPayeeRuleHandler payeeRuleHandler)
    {
        var idOption = new Option<int>("--id")
        {
            Description = "Payee Rule Id",
            Required = true,
        };

        var deletePayeeRuleCommand = new Command("delete", "Delete existing payee rule")
        {
            idOption,
        };

        deletePayeeRuleCommand.SetAction((parseResult, cancellationToken) =>
            payeeRuleHandler.DeleteAsync(
                parseResult.GetRequiredValue(idOption),
                cancellationToken));

        return deletePayeeRuleCommand;
    }
}