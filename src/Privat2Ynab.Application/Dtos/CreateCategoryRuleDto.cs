using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Dtos;

public sealed record CreateCategoryRuleDto(
    string Memo,
    StringMatchType MatchType,
    Guid CategoryGroupId,
    Guid CategoryId);