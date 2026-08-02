using System.CommandLine;

using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Console.Commands;

internal static class CategoryRuleCommands
{
    extension(RootCommand rootCommand)
    {
        public RootCommand AddCategoryRuleCommands(IServiceProvider serviceProvider)
        {
            var categoryRuleHandler = serviceProvider.GetRequiredService<ICategoryRuleHandler>();

            rootCommand.Add(
                new Command("category-rules")
                {
                    CreateListCategoryRulesCommand(categoryRuleHandler),
                    CreateAddCategoryRuleCommand(categoryRuleHandler),
                    CreateSynchronizeRulesCommand(categoryRuleHandler),
                    ////CreateUpdateCategoryRuleCommand(), // TODO: Decide if needed
                    CreateDeleteCategoryRuleCommand(categoryRuleHandler),
                });

            return rootCommand;
        }
    }

    private static Command CreateListCategoryRulesCommand(ICategoryRuleHandler categoryRuleHandler)
    {
        var listCategoryRulesCommand = new Command("list", "List category rules");
        listCategoryRulesCommand.SetAction((_, cancellationToken) => categoryRuleHandler.ListAsync(cancellationToken));

        return listCategoryRulesCommand;
    }

    private static Command CreateAddCategoryRuleCommand(ICategoryRuleHandler categoryRuleHandler)
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
        var categoryGroupIdOption = new Option<string>("--category-group-name")
        {
            Description = "YNAB Category Group Name",
            Required = true,
        };
        var categoryIdOption = new Option<string>("--category-name")
        {
            Description = "YNAB Category Name",
            Required = true,
        };

        var addCategoryRuleCommand = new Command("add", "Add new category rule")
        {
            planIdOption,
            memoOption,
            matchTypeOption,
            categoryGroupIdOption,
            categoryIdOption,
        };

        addCategoryRuleCommand.SetAction((parseResult, cancellationToken) =>
            categoryRuleHandler.AddAsync(
                new CreateCategoryRuleDto(
                    parseResult.GetRequiredValue(planIdOption),
                    parseResult.GetRequiredValue(memoOption),
                    parseResult.GetRequiredValue(matchTypeOption),
                    parseResult.GetRequiredValue(categoryGroupIdOption),
                    parseResult.GetRequiredValue(categoryIdOption)),
                cancellationToken));

        return addCategoryRuleCommand;
    }

    private static Command CreateSynchronizeRulesCommand(ICategoryRuleHandler categoryRuleHandler)
    {
        var planIdOption = new Option<int?>("--plan-id")
        {
            Description = "Plan Id"
        };

        var addPayeeRuleCommand = new Command("sync", "Synchronize payee names with YNAB")
        {
            planIdOption
        };

        addPayeeRuleCommand.SetAction((parseResult, cancellationToken) =>
            categoryRuleHandler.SynchronizeAsync(
                new FilterDto(
                    parseResult.GetValue(planIdOption)),
                cancellationToken));

        return addPayeeRuleCommand;
    }

    private static Command CreateDeleteCategoryRuleCommand(ICategoryRuleHandler categoryRuleHandler)
    {
        var idOption = new Option<int>("--id")
        {
            Description = "Category Rule Id",
            Required = true,
        };

        var deleteCategoryRuleCommand = new Command("delete", "Delete existing category rule")
        {
            idOption,
        };

        deleteCategoryRuleCommand.SetAction((parseResult, cancellationToken) =>
            categoryRuleHandler.DeleteAsync(
                parseResult.GetRequiredValue(idOption),
                cancellationToken));

        return deleteCategoryRuleCommand;
    }
}