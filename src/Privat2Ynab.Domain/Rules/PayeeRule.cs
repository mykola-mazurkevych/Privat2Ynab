using Privat2Ynab.Domain.Plans;

namespace Privat2Ynab.Domain.Rules;

public sealed class PayeeRule :
    IEntity
{
    private PayeeRule()
    {
    }

    private PayeeRule(DateTimeOffset createdAt, int planId, string memo, StringMatchType matchType, Guid ynabId, string name) =>
        (CreatedAt, PlanId, Memo, MatchType, YnabId, Name) = (createdAt, planId, memo, matchType, ynabId, name);

    public int Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public int PlanId { get; private set; }
    public Plan Plan { get; private set; } = null!;

    public string Memo { get; private set; } = null!;

    public StringMatchType MatchType { get; private set; }

    public Guid YnabId { get; private set; }
    public string Name { get; private set; } = null!;

    public static PayeeRule Create(
        DateTimeOffset createdAt,
        int planId,
        string memo,
        StringMatchType matchType,
        Guid ynabId,
        string name) =>
        new(createdAt, planId, memo, matchType, ynabId, name);

    public bool IsApplicableTo(string? memo) =>
        !string.IsNullOrEmpty(memo) && MatchType switch
        {
            StringMatchType.Exact => string.Equals(memo, Memo, StringComparison.OrdinalIgnoreCase),
            StringMatchType.StartsWith => memo.StartsWith(Memo, StringComparison.OrdinalIgnoreCase),
            StringMatchType.EndsWith => memo.EndsWith(Memo, StringComparison.OrdinalIgnoreCase),
            StringMatchType.Contains => memo.Contains(Memo, StringComparison.OrdinalIgnoreCase),
            _ => throw new NotSupportedException($"String match type {MatchType} is not supported"),
        };
}