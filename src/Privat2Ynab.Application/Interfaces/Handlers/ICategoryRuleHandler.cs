using Privat2Ynab.Application.Dtos;

namespace Privat2Ynab.Application.Interfaces.Handlers;

public interface ICategoryRuleHandler
{
    Task ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CreateCategoryRuleDto create, CancellationToken cancellationToken = default);
    Task SynchronizeAsync(FilterDto filter, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}