using Privat2Ynab.Application.Dtos.Ynab;

namespace Privat2Ynab.Application.Interfaces.Services;

public interface IYnabClient
{
    Task<YnabPlan?> GetPlanAsync(Guid planId, string token, CancellationToken cancellationToken = default);
    Task<YnabAccount?> GetAccountAsync(Guid planId, Guid accountId, string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<YnabPayee>> GetPayeesAsync(Guid planId, string token, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<YnabCategoryGroup>> GetCategoryGroupsAsync(Guid planId, string token, CancellationToken cancellationToken = default);
}