namespace Privat2Ynab.Domain.Accounts;

public sealed record Account :
    IEntity
{
    private Account()
    {
    }

    public int Id { get; private set; }

    public string PersonalAccessToken { get; private set; } = null!;
    public Guid BudgetId { get; private set; }
    public string BudgetName { get; private set; } = null!;
    public Guid AccountId { get; private set; }
    public string AccountName { get; private set; } = null!;

    public string FileName { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public static Account Create(
        string personalAccessToken,
        Guid budgetId,
        string budgetName,
        Guid accountId,
        string accountName,
        string fileName,
        bool isActive) =>
        new()
        {
            PersonalAccessToken = personalAccessToken,
            BudgetId = budgetId,
            BudgetName = budgetName,
            AccountId = accountId,
            AccountName = accountName,
            FileName = fileName,
            IsActive = isActive,
        };
}