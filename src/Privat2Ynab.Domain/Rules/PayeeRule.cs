namespace Privat2Ynab.Domain.Rules;

public sealed record PayeeRule
{
    private PayeeRule()
    {
    }

    public Guid Id { get; private set; }

    public string Memo { get; private set; } = null!;

    public RuleType Type { get; private set; }

    public string PayeeName { get; private set; } = null!;

    public static PayeeRule Create(
        Guid id,
        string memo,
        RuleType type,
        string payeeName) =>
        new()
        {
            Id = id,
            Memo = memo,
            Type = type,
            PayeeName = payeeName,
        };
}