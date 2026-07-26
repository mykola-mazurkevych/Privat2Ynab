namespace Privat2Ynab.Domain.Rules;

public sealed record CategoryRule :
    IEntity
{
    private CategoryRule()
    {
    }

    public int Id { get; private set; }

    public string Memo { get; private set; } = null!;

    public StringMatchType MatchType { get; private set; }

    public Guid CategoryGroupId { get; private set; }
    public string CategoryGroupName { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public string CategoryName { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public static CategoryRule Create(
        string memo,
        StringMatchType matchType,
        Guid categoryGroupId,
        string categoryGroupName,
        Guid categoryId,
        string categoryName,
        bool isActive) =>
        new()
        {
            Memo = memo,
            MatchType = matchType,
            CategoryGroupId = categoryGroupId,
            CategoryGroupName = categoryGroupName,
            CategoryId = categoryId,
            CategoryName = categoryName,
            IsActive = isActive,
        };
}