namespace Privat2Ynab.Domain.Rules;

public sealed record PayeeRule :
    IEntity
{
    private PayeeRule()
    {
    }

    public int Id { get; private set; }

    public string Memo { get; private set; } = null!;

    public RuleType Type { get; private set; }

    public string PayeeName { get; private set; } = null!;

    public static PayeeRule Create(
        string memo,
        RuleType type,
        string payeeName) =>
        new()
        {
            Memo = memo,
            Type = type,
            PayeeName = payeeName,
        };
}