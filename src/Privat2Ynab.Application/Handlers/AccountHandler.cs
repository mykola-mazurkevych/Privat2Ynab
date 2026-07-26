using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Domain.Accounts;

namespace Privat2Ynab.Application.Handlers;

internal sealed class AccountHandler(
    IOutputWriter outputWriter,
    IRepository repository) :
    IAccountHandler
{
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await repository.ListAsync<Account>(cancellationToken);
        outputWriter.Write(accounts.Select(AccountModel.Create).ToTable(headless: false));
    }

    public async Task AddAsync(CreateAccountDto createAccount, CancellationToken cancellationToken = default)
    {
        var account = Account.Create(
            createAccount.FileName,
            createAccount.YnabAccountId);
        account = await repository.AddAsync(account, cancellationToken);
        outputWriter.Write("Account added:");
        outputWriter.Write(AccountModel.Create(account).ToTable());
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<Account>(id, cancellationToken);
        outputWriter.Write($"Account {id} deleted");
    }

    private sealed record AccountModel(
        int Id,
        [property: DisplayName("File Name")] string FileName,
        [property: DisplayName("YNAB Account Id")] Guid YnabAccountId)
    {
        public static AccountModel Create(Account account) =>
            new(account.Id,
                account.FileName,
                account.YnabAccountId);
    }
}