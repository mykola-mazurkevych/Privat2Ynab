using Privat2Ynab.Application.Dtos;

namespace Privat2Ynab.Application.Interfaces.Handlers;

public interface IAccountHandler
{
    Task ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CreateAccountDto createAccount, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}