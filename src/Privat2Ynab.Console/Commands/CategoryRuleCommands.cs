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
        var categoryGroupIdOption = new Option<Guid>("--category-group-id")
        {
            Description = "YNAB Category Group Id",
            Required = true,
        };
        var categoryIdOption = new Option<Guid>("--category-id")
        {
            Description = "YNAB Category Id",
            Required = true,
        };

        var addCategoryRuleCommand = new Command("add", "Add new category rule")
        {
            memoOption,
            matchTypeOption,
            categoryGroupIdOption,
            categoryIdOption,
        };

        addCategoryRuleCommand.SetAction((parseResult, cancellationToken) =>
            categoryRuleHandler.AddAsync(
                new CreateCategoryRuleDto(
                    parseResult.GetRequiredValue(memoOption),
                    parseResult.GetRequiredValue(matchTypeOption),
                    parseResult.GetRequiredValue(categoryGroupIdOption),
                    parseResult.GetRequiredValue(categoryIdOption)),
                cancellationToken));

        return addCategoryRuleCommand;
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