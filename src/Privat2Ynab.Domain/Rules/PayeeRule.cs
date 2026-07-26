namespace Privat2Ynab.Domain.Rules;

public sealed record PayeeRule :
    IEntity
{
    private PayeeRule()
    {
    }

    public int Id { get; private set; }

    public string Memo { get; private set; } = null!;

    public StringMatchType MatchType { get; private set; }

    public Guid PayeeId { get; private set; }
    public string PayeeName { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public static PayeeRule Create(
        string memo,
        StringMatchType type,
        Guid payeeId,
        string payeeName,
        bool isActive) =>
        new()
        {
            Memo = memo,
            MatchType = type,
            PayeeId = payeeId,
            PayeeName = payeeName,
            IsActive = isActive,
        };
}