// ReSharper disable UnusedMember.Global

using Privat2Ynab.Domain.Plans;

namespace Privat2Ynab.Domain.Rules;

public sealed class CategoryRule :
    IEntity
{
    private CategoryRule()
    {
    }

    public int Id { get; private set; }

    public int PlanId { get; private set; }
    public Plan Plan { get; private set; } = null!;

    public string Memo { get; private set; } = null!;

    public StringMatchType MatchType { get; private set; }

    public Guid YnabId { get; private set; }
    public string Name { get; private set; } = null!;

    public static CategoryRule Create(
        int planId,
        string memo,
        StringMatchType matchType,
        Guid ynabId,
        string name) =>
        new()
        {
            PlanId = planId,
            Memo = memo,
            MatchType = matchType,
            YnabId = ynabId,
            Name = name,
        };
}