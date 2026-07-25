namespace Privat2Ynab.Domain.Rules;

public sealed record CategoryRule
{
    private CategoryRule()
    {
    }

    public Guid Id { get; private set; }

    public string Memo { get; private set; } = null!;

    public RuleType Type { get; private set; }

    public string CategoryGroupName { get; private set; } = null!;
    public string CategoryName { get; private set; } = null!;

    public static CategoryRule Create(
        Guid id,
        string memo,
        RuleType type,
        string categoryGroupName,
        string categoryName) =>
        new()
        {
            Id = id,
            Memo = memo,
            Type = type,
            CategoryGroupName = categoryGroupName,
            CategoryName = categoryName,
        };
}