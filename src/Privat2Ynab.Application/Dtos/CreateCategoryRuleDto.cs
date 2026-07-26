using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Dtos;

public sealed record CreateCategoryRuleDto(
    string Memo,
    RuleType Type,
    string CategoryGroupName,
    string CategoryName);