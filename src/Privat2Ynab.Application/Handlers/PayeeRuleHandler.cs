using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
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
        var payeeRules = await repository.ListAsync<PayeeRule>(cancellationToken);
        output.WriteLine(payeeRules.Select(PayeeRuleModel.Create).ToTable(headless: false));
    }

    public async Task AddAsync(CreatePayeeRuleDto createPayeeRule, CancellationToken cancellationToken = default)
    {
        var plan = await repository.GetAsync<Plan>(createPayeeRule.PlanId, cancellationToken)
                   ?? throw new InvalidOperationException("Plan not found");

        var ynabPayees = await ynabClient.GetPayeesAsync(plan.YnabId, plan.Token, cancellationToken);
        var ynabPayee = ynabPayees.SingleOrDefault(p => string.Equals(p.Name, createPayeeRule.PayeeName, StringComparison.OrdinalIgnoreCase))
                        ?? throw new InvalidOperationException("Payee not found");

        var payeeRule = PayeeRule.Create(
            plan.Id,
            createPayeeRule.Memo,
            createPayeeRule.MatchType,
            ynabPayee.Id,
            ynabPayee.Name);
        payeeRule = await repository.AddAsync(payeeRule, cancellationToken);
        output.WriteLine("Payee rule added:");
        output.WriteLine(PayeeRuleModel.Create(payeeRule).ToTable());
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<PayeeRule>(id, cancellationToken);
        output.WriteLine($"Payee rule {id} deleted");
    }

    private sealed record PayeeRuleModel(
        int Id,
        [property: DisplayName("Plan Id")] int PlanId,
        [property: DisplayName("Plan Name")] string PlanName,
        string Memo,
        [property: DisplayName("String Match Type")] StringMatchType MatchType,
        [property: DisplayName("YNAB Payee Id")] Guid PayeeId,
        [property: DisplayName("YNAB Payee Name")] string PayeeName)
    {
        public static PayeeRuleModel Create(PayeeRule payeeRule) =>
            new(payeeRule.Id,
                payeeRule.Plan.Id,
                payeeRule.Plan.Name,
                payeeRule.Memo,
                payeeRule.MatchType,
                payeeRule.YnabId,
                payeeRule.Name);
    }
}