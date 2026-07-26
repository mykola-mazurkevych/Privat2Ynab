using Privat2Ynab.Application.Dtos;

namespace Privat2Ynab.Application.Interfaces.Handlers;

public interface IPlanHandler
{
    Task ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CreatePlanDto createPlan, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}