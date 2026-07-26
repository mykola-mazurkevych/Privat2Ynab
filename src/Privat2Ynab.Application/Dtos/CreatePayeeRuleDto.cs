using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Dtos;

public sealed record CreatePayeeRuleDto(
    string Memo,
    StringMatchType MatchType,
    Guid PayeeId);