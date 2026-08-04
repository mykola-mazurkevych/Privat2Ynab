using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Handlers.Models;

internal sealed record PayeeRuleModel(
    int Id,
    [property: DisplayName("Plan Id")] int PlanId,
    [property: DisplayName("Plan Name")] string PlanName,
    string Memo,
    [property: DisplayName("String Match Type")] StringMatchType MatchType,
    [property: DisplayName("YNAB Payee Id")] Guid PayeeId,
    [property: DisplayName("YNAB Payee Name")] string Name,
    [property: DisplayName("Created At")] DateTime CreatedAt)
{
    public static PayeeRuleModel Create(PayeeRule payeeRule) =>
        new(payeeRule.Id,
            payeeRule.Plan.Id,
            payeeRule.Plan.Name,
            payeeRule.Memo,
            payeeRule.MatchType,
            payeeRule.YnabId,
            payeeRule.Name,
            payeeRule.CreatedAt.ToLocalTime().DateTime);
}