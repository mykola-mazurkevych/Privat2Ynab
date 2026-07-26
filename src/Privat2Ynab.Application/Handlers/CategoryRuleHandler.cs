using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Handlers;

internal sealed class CategoryRuleHandler(
    IOutputWriter outputWriter,
    IRepository repository) :
    ICategoryRuleHandler
{
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var categoryRules = await repository.ListAsync<CategoryRule>(cancellationToken);
        outputWriter.Write(categoryRules.Select(CategoryRuleModel.Create).ToTable(headless: false));
    }

    public async Task AddAsync(CreateCategoryRuleDto createCategoryRule, CancellationToken cancellationToken = default)
    {
        var categoryRule = CategoryRule.Create(
            createCategoryRule.Memo,
            createCategoryRule.Type,
            createCategoryRule.CategoryGroupName,
            createCategoryRule.CategoryName);
        categoryRule = await repository.AddAsync(categoryRule, cancellationToken);
        outputWriter.Write("Category rule added:");
        outputWriter.Write(CategoryRuleModel.Create(categoryRule).ToTable());
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<CategoryRule>(id, cancellationToken);
        outputWriter.Write($"Category rule {id} deleted");
    }

    private sealed record CategoryRuleModel(
        int Id,
        string Memo,
        RuleType Type,
        [property: DisplayName("Category Group Name")] string CategoryGroupName,
        [property: DisplayName("Category Name")] string CategoryName)
    {
        public static CategoryRuleModel Create(CategoryRule categoryRule) =>
            new(categoryRule.Id,
                categoryRule.Memo,
                categoryRule.Type,
                categoryRule.CategoryGroupName,
                categoryRule.CategoryName);
    }
}