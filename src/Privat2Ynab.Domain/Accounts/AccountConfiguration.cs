namespace Privat2Ynab.Domain.Accounts;

public sealed record AccountConfiguration
{
    private AccountConfiguration()
    {
    }

    public Guid Id { get; private set; }

    public string FileName { get; private set; } = null!;
    public Guid YnabAccountId { get; private set; }

    public static AccountConfiguration Create(
        Guid id,
        string fileName,
        Guid ynabAccountId) =>
        new()
        {
            Id = id,
            FileName = fileName,
            YnabAccountId = ynabAccountId,
        };
}