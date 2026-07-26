using Privat2Ynab.Application.Dtos;

namespace Privat2Ynab.Application.Interfaces.Handlers;

public interface IPayeeRuleHandler
{
    Task ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CreatePayeeRuleDto createCategoryRule, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}