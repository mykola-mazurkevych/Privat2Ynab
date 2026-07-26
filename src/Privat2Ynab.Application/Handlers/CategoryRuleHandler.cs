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
            createCategoryRule.MatchType,
            createCategoryRule.CategoryGroupId,
            "", // TODO: enrich category group name
            createCategoryRule.CategoryId,
            "", // TODO: enrich category name
            isActive: true);
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
        [property: DisplayName("String Match Type")] StringMatchType MatchType,
        [property: DisplayName("YNAB Category Group Id")] Guid CategoryGroupId,
        [property: DisplayName("YNAB Category Group Name")] string CategoryGroupName,
        [property: DisplayName("YNAB Group Id")] Guid CategoryId,
        [property: DisplayName("YNAB Category Name")] string CategoryName)
    {
        public static CategoryRuleModel Create(CategoryRule categoryRule) =>
            new(categoryRule.Id,
                categoryRule.Memo,
                categoryRule.MatchType,
                categoryRule.CategoryGroupId,
                categoryRule.CategoryGroupName,
                categoryRule.CategoryId,
                categoryRule.CategoryName);
    }
}