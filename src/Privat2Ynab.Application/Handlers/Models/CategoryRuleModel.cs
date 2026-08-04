using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Handlers.Models;

internal sealed record CategoryRuleModel(
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