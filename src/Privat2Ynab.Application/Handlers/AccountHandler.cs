using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Handlers.Models;
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
        var accounts = await repository.GetAllAsync<Account>(cancellationToken);
        output.WriteLine(accounts.Select(AccountModel.Create).OrderBy(a => a.Name).ToTable(headless: false));
    }

    public async Task AddAsync(CreateAccountDto createAccount, CancellationToken cancellationToken = default)
    {
        var plan =
            await repository.GetAsync<Plan>(createAccount.PlanId, cancellationToken)
            ?? throw new InvalidOperationException("Plan not found");

        var ynabAccount =
            await ynabClient.GetAccountAsync(plan.YnabId, createAccount.YnabId, plan.Token, cancellationToken)
            ?? throw new InvalidOperationException("Account not found");

        var account = Account.Create(
            DateTimeOffset.UtcNow,
            plan.Id,
            ynabAccount.Id,
            ynabAccount.Name,
            createAccount.FileName);
        account = await repository.AddAsync(account, cancellationToken);
        output.WriteLine("Account added:");
        output.WriteLine(AccountModel.Create(account).ToTable());
    }

    public async Task SynchronizeAsync(FilterDto filter, CancellationToken cancellationToken = default)
    {
        var plans = filter.PlanId.HasValue
            ? [await repository.GetAsync<Plan>(filter.PlanId.Value, cancellationToken) ?? throw new InvalidOperationException("Plan not found")]
            : await repository.GetAllAsync<Plan>(cancellationToken);

        foreach (var plan in plans)
        {
            var accounts = await repository.GetAllAsync<Account>(a => a.PlanId == plan.Id, cancellationToken);
            if (accounts.Count == 0)
            {
                continue;
            }

            var ynabAccounts = await ynabClient.GetAccountsAsync(plan.YnabId, plan.Token, cancellationToken);
            var ynabAccountIdToNameMap = ynabAccounts
                .ToDictionary(a => a.Id, a => a.Name);
            
            var accountsToUpdate = new List<Account>(accounts.Count);
            var accountsToDelete = new List<Account>(accounts.Count);

            foreach (var account in accounts)
            {
                if (!ynabAccountIdToNameMap.TryGetValue(account.YnabId, out var name))
                {
                    accountsToDelete.Add(account);
                }
                else if (!string.Equals(account.Name, name, StringComparison.Ordinal))
                {
                    account.UpdateName(name);
                    accountsToUpdate.Add(account);
                }
            }

            await repository.UpdateAsync(accountsToUpdate.AsReadOnly(), cancellationToken);
            await repository.DeleteAsync(accountsToDelete.AsReadOnly(), cancellationToken);

            output.WriteLine(new SyncModel(plan.Id, plan.Name, accountsToUpdate.Count, accountsToDelete.Count).ToTable());
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<Account>(id, cancellationToken);
        output.WriteLine($"Account {id} deleted");
    }
}