using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Dtos;

public sealed record CreateCategoryRuleDto(
    int PlanId,
    string Memo,
    StringMatchType MatchType,
    string CategoryGroupName,
    string CategoryName);