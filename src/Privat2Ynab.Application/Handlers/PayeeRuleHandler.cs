using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Handlers.Models;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Domain.Plans;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Handlers;

internal sealed class PayeeRuleHandler(
    IOutput output,
    IRepository repository,
    IYnabClient ynabClient) :
    IPayeeRuleHandler
{
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var payeeRules = await repository.GetAllAsync<PayeeRule>(cancellationToken);
        output.WriteLine(payeeRules.Select(PayeeRuleModel.Create).OrderBy(p => p.Name).ToTable(headless: false));
    }

    public async Task AddAsync(CreatePayeeRuleDto create, CancellationToken cancellationToken = default)
    {
        var plan = await repository.GetAsync<Plan>(create.PlanId, cancellationToken)
                   ?? throw new InvalidOperationException("Plan not found");

        var ynabPayees = await ynabClient.GetPayeesAsync(plan.YnabId, plan.Token, cancellationToken);
        var ynabPayee = ynabPayees.SingleOrDefault(p => string.Equals(p.Name, create.PayeeName, StringComparison.OrdinalIgnoreCase))
                        ?? throw new InvalidOperationException("Payee not found");

        var payeeRule = PayeeRule.Create(
            DateTimeOffset.UtcNow,
            plan.Id,
            create.Memo,
            create.MatchType,
            ynabPayee.Id,
            ynabPayee.Name);
        payeeRule = await repository.AddAsync(payeeRule, cancellationToken);
        output.WriteLine("Payee rule added:");
        output.WriteLine(PayeeRuleModel.Create(payeeRule).ToTable());
    }

    public async Task SynchronizeAsync(FilterDto filter, CancellationToken cancellationToken = default)
    {
        var plans = filter.PlanId.HasValue
            ? [await repository.GetAsync<Plan>(filter.PlanId.Value, cancellationToken) ?? throw new InvalidOperationException("Plan not found")]
            : await repository.GetAllAsync<Plan>(cancellationToken);

        foreach (var plan in plans)
        {
            var payeeRules = await repository.GetAllAsync<PayeeRule>(p => p.PlanId == plan.Id, cancellationToken);
            if (payeeRules.Count == 0)
            {
                continue;
            }

            var ynabPayees = await ynabClient.GetPayeesAsync(plan.YnabId, plan.Token, cancellationToken);
            var ynabPayeeIdToNameMap = ynabPayees.ToDictionary(p => p.Id, p => p.Name);

            var payeeRulesToUpdate = new List<PayeeRule>(payeeRules.Count);
            var payeeRulesToDelete = new List<PayeeRule>(payeeRules.Count);

            foreach (var payeeRule in payeeRules)
            {
                if (!ynabPayeeIdToNameMap.TryGetValue(payeeRule.YnabId, out var name))
                {
                    payeeRulesToDelete.Add(payeeRule);
                }
                else if (!string.Equals(payeeRule.Name, name, StringComparison.Ordinal))
                {
                    payeeRule.UpdateName(name);
                    payeeRulesToUpdate.Add(payeeRule);
                }
            }

            await repository.UpdateAsync(payeeRulesToUpdate.AsReadOnly(), cancellationToken);
            await repository.DeleteAsync(payeeRulesToDelete.AsReadOnly(), cancellationToken);

            output.WriteLine(new SyncModel(plan.Id, plan.Name, payeeRulesToUpdate.Count, payeeRulesToDelete.Count).ToTable());
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<PayeeRule>(id, cancellationToken);
        output.WriteLine($"Payee rule {id} deleted");
    }
}