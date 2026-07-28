using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Domain.Accounts;
using Privat2Ynab.Domain.Plans;

namespace Privat2Ynab.Application.Handlers;

internal sealed class AccountHandler(
    IOutput output,
    IRepository repository,
    IYnabClient ynabClient) :
    IAccountHandler
{
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await repository.ListAsync<Account>(cancellationToken);
        output.WriteLine(accounts.Select(AccountModel.Create).ToTable(headless: false));
    }

    public async Task AddAsync(CreateAccountDto createAccount, CancellationToken cancellationToken = default)
    {
        var plan = await repository.GetAsync<Plan>(createAccount.PlanId, cancellationToken)
                   ?? throw new InvalidOperationException("Plan not found");

        var ynabAccount = await ynabClient.GetAccountAsync(plan.YnabId, createAccount.YnabId, plan.Token, cancellationToken)
                          ?? throw new InvalidOperationException("Account not found");

        var account = Account.Create(
            plan.Id,
            ynabAccount.Id,
            ynabAccount.Name,
            createAccount.FileName);
        account = await repository.AddAsync(account, cancellationToken);
        output.WriteLine("Account added:");
        output.WriteLine(AccountModel.Create(account).ToTable());
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<Account>(id, cancellationToken);
        output.WriteLine($"Account {id} deleted");
    }

    private sealed record AccountModel(
        int Id,
        [property: DisplayName("Plan Id")] int PlanId,
        [property: DisplayName("Plan Name")] string PlanName,
        [property: DisplayName("YNAB Account Id")] Guid YnabId,
        [property: DisplayName("YNAB Account Name")] string Name,
        [property: DisplayName("File Name")] string FileName)
    {
        public static AccountModel Create(Account account) =>
            new(account.Id,
                account.Plan.Id,
                account.Plan.Name,
                account.YnabId,
                account.Name,
                account.FileName);
    }
}