using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Domain.Plans;

namespace Privat2Ynab.Application.Handlers.Models;

internal sealed record PlanModel(
    int Id,
    [property: DisplayName("YNAB Plan Id")] Guid YnabId,
    [property: DisplayName("YNAB Plan Name")] string Name,
    [property: DisplayName("Created At")] DateTime CreatedAt)
{
    public static PlanModel Create(Plan plan) =>
        new(plan.Id,
            plan.YnabId,
            plan.Name,
            plan.CreatedAt.ToLocalTime().DateTime);
}