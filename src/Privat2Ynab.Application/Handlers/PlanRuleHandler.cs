using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Domain.Plans;

namespace Privat2Ynab.Application.Handlers;

internal sealed class PlanHandler(
    IOutput output,
    IRepository repository,
    IYnabClient ynabClient) :
    IPlanHandler
{
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var plans = await repository.GetAllAsync<Plan>(cancellationToken);
        output.WriteLine(plans.Select(PlanModel.Create).OrderBy(p => p.Name).ToTable(headless: false));
    }

    public async Task AddAsync(CreatePlanDto createPlan, CancellationToken cancellationToken = default)
    {
        var ynabPlan = await ynabClient.GetPlanAsync(createPlan.PlanId, createPlan.Token, cancellationToken) ??
                       throw new InvalidOperationException("Plan not found");

        var plan = Plan.Create(
            DateTimeOffset.UtcNow,
            ynabPlan.Id,
            ynabPlan.Name,
            createPlan.Token);
        plan = await repository.AddAsync(plan, cancellationToken);
        output.WriteLine("Plan added:");
        output.WriteLine(PlanModel.Create(plan).ToTable());
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<Plan>(id, cancellationToken);
        output.WriteLine($"Plan {id} deleted");
    }

    private sealed record PlanModel(
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
}