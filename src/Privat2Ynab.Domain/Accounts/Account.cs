namespace Privat2Ynab.Domain.Accounts;

public sealed record Account
{
    private Account()
    {
    }

    public int Id { get; private set; }

    public string FileName { get; private set; } = null!;
    public Guid YnabAccountId { get; private set; }

    public static Account Create(
        string fileName,
        Guid ynabAccountId) =>
        new()
        {
            FileName = fileName,
            YnabAccountId = ynabAccountId,
        };
}