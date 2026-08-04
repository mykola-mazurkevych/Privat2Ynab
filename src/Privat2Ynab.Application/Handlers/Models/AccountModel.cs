using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Domain.Accounts;

namespace Privat2Ynab.Application.Handlers.Models;

internal sealed record AccountModel(
    int Id,
    [property: DisplayName("Plan Id")] int PlanId,
    [property: DisplayName("Plan Name")] string PlanName,
    [property: DisplayName("YNAB Account Id")] Guid YnabId,
    [property: DisplayName("YNAB Account Name")] string Name,
    [property: DisplayName("File Name")] string FileName,
    [property: DisplayName("Created At")] DateTime CreatedAt)
{
    public static AccountModel Create(Account account) =>
        new(account.Id,
            account.Plan.Id,
            account.Plan.Name,
            account.YnabId,
            account.Name,
            account.FileName,
            account.CreatedAt.ToLocalTime().DateTime);
}