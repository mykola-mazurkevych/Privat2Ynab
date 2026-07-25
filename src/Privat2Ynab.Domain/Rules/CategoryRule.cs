namespace Privat2Ynab.Domain.Rules;

public sealed record CategoryRule
{
    private CategoryRule()
    {
    }

    public int Id { get; private set; }

    public string Memo { get; private set; } = null!;

    public RuleType Type { get; private set; }

    public string CategoryGroupName { get; private set; } = null!;
    public string CategoryName { get; private set; } = null!;

    public static CategoryRule Create(
        string memo,
        RuleType type,
        string categoryGroupName,
        string categoryName) =>
        new()
        {
            Memo = memo,
            Type = type,
            CategoryGroupName = categoryGroupName,
            CategoryName = categoryName,
        };
}