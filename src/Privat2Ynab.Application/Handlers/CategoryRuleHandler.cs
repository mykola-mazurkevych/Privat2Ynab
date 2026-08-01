using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Domain.Plans;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Handlers;

internal sealed class CategoryRuleHandler(
    IOutput output,
    IRepository repository,
    IYnabClient ynabClient) :
    ICategoryRuleHandler
{
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var categoryRules = await repository.GetAllAsync<CategoryRule>(cancellationToken);
        output.WriteLine(categoryRules.Select(CategoryRuleModel.Create).OrderBy(c => c.Name).ToTable(headless: false));
    }

    public async Task AddAsync(CreateCategoryRuleDto createCategoryRule, CancellationToken cancellationToken = default)
    {
        var plan = await repository.GetAsync<Plan>(createCategoryRule.PlanId, cancellationToken)
                   ?? throw new InvalidOperationException("Plan not found");

        var ynabCategoryGroups = await ynabClient.GetCategoryGroupsAsync(plan.YnabId, plan.Token, cancellationToken);
        var ynabCategoryGroup = ynabCategoryGroups.SingleOrDefault(g => string.Equals(g.Name, createCategoryRule.CategoryGroupName, StringComparison.OrdinalIgnoreCase))
                                ?? throw new InvalidOperationException("Category group not found");
        var ynabCategory = ynabCategoryGroup.Categories.SingleOrDefault(c => string.Equals(c.Name, createCategoryRule.CategoryName, StringComparison.OrdinalIgnoreCase))
                           ?? throw new InvalidOperationException("Category not found");

        var categoryRule = CategoryRule.Create(
            DateTimeOffset.UtcNow,
            plan.Id,
            createCategoryRule.Memo,
            createCategoryRule.MatchType,
            ynabCategory.Id,
            $"{ynabCategoryGroup.Name} => {ynabCategory.Name}");
        categoryRule = await repository.AddAsync(categoryRule, cancellationToken);
        output.WriteLine("Category rule added:");
        output.WriteLine(CategoryRuleModel.Create(categoryRule).ToTable());
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<CategoryRule>(id, cancellationToken);
        output.WriteLine($"Category rule {id} deleted");
    }

    private sealed record CategoryRuleModel(
        int Id,
        [property: DisplayName("Plan Id")] int PlanId,
        [property: DisplayName("Plan Name")] string PlanName,
        string Memo,
        [property: DisplayName("String Match Type")] StringMatchType MatchType,
        [property: DisplayName("YNAB Category Id")] Guid YnabId,
        [property: DisplayName("YNAB Category Name")] string Name,
        [property: DisplayName("Created At")] DateTime CreatedAt)
    {
        public static CategoryRuleModel Create(CategoryRule categoryRule) =>
            new(categoryRule.Id,
                categoryRule.Plan.Id,
                categoryRule.Plan.Name,
                categoryRule.Memo,
                categoryRule.MatchType,
                categoryRule.YnabId,
                categoryRule.Name,
                categoryRule.CreatedAt.ToLocalTime().DateTime);
    }
}