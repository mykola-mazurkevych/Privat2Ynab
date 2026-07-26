using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Dtos;

public sealed record CreatePayeeRuleDto(
    int PlanId,
    string Memo,
    StringMatchType MatchType,
    string PayeeName);