using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Handlers;

internal sealed class PayeeRuleHandler(
    IOutputWriter outputWriter,
    IRepository repository) :
    IPayeeRuleHandler
{
    public async Task ListAsync(CancellationToken cancellationToken = default)
    {
        var payeeRules = await repository.ListAsync<PayeeRule>(cancellationToken);
        outputWriter.Write(payeeRules.Select(PayeeRuleModel.Create).ToTable(headless: false));
    }

    public async Task AddAsync(CreatePayeeRuleDto createPayeeRule, CancellationToken cancellationToken = default)
    {
        var payeeRule = PayeeRule.Create(
            createPayeeRule.Memo,
            createPayeeRule.MatchType,
            createPayeeRule.PayeeId,
            "", // TODO: enrich payee name
            isActive: true);
        payeeRule = await repository.AddAsync(payeeRule, cancellationToken);
        outputWriter.Write("Payee rule added:");
        outputWriter.Write(PayeeRuleModel.Create(payeeRule).ToTable());
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync<PayeeRule>(id, cancellationToken);
        outputWriter.Write($"Payee rule {id} deleted");
    }

    private sealed record PayeeRuleModel(
        int Id,
        string Memo,
        [property: DisplayName("String Match Type")] StringMatchType MatchType,
        [property: DisplayName("YNAB Payee Id")] Guid PayeeId,
        [property: DisplayName("YNAB Payee Name")] string PayeeName)
    {
        public static PayeeRuleModel Create(PayeeRule payeeRule) =>
            new(payeeRule.Id,
                payeeRule.Memo,
                payeeRule.MatchType,
                payeeRule.PayeeId,
                payeeRule.PayeeName);
    }
}