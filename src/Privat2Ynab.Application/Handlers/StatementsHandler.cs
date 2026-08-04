using Privat2Ynab.Application.Attributes;
using Privat2Ynab.Application.Dtos.Ynab;
using Privat2Ynab.Application.Extensions;
using Privat2Ynab.Application.Interfaces.Handlers;
using Privat2Ynab.Application.Interfaces.Persistence;
using Privat2Ynab.Application.Interfaces.Services;
using Privat2Ynab.Domain.Accounts;
using Privat2Ynab.Domain.Rules;

namespace Privat2Ynab.Application.Handlers;

internal sealed class StatementsHandler(
    IOutput output,
    IRepository repository,
    IStatementsReader statementsReader,
    IYnabClient ynabClient) :
    IStatementsHandler
{
    private const string InputDirectoryName = "input";

    private const int YnabDecimalMultiplier = 1000;

    public async Task HandleAsync(CancellationToken cancellationToken = default)
    {
        var inputDirectoryInfo = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, InputDirectoryName));
        if (!inputDirectoryInfo.Exists)
        {
            inputDirectoryInfo.Create();
        }

        List<FileInfo> fileInfos = [.. inputDirectoryInfo.EnumerateFiles()];
        if (fileInfos.Count == 0)
        {
            return;
        }

        var accounts = await repository.GetAllAsync<Account>(cancellationToken);
        var fileNameToYnabAccountMap = accounts
            .ToDictionary(
                a => a.FileName,
                StringComparer.OrdinalIgnoreCase);

        var allCategoryRules = await repository.GetAllAsync<CategoryRule>(cancellationToken);
        var planIdToCategoryRulesMap = allCategoryRules
            .GroupBy(c => c.PlanId)
            .ToDictionary(g => g.Key, g => g.ToList());
        var allPayeeRules = await repository.GetAllAsync<PayeeRule>(cancellationToken);
        var planIdToPayeeRulesMap = allPayeeRules
            .GroupBy(p => p.PlanId)
            .ToDictionary(g => g.Key, g => g.ToList());

        List<ResultModel> results = [];

        foreach (var fileInfo in fileInfos)
        {
            output.WriteLine($"Processing file {fileInfo.Name}...");

            if (!fileNameToYnabAccountMap.TryGetValue(fileInfo.Name, out var account))
            {
                output.WriteLine("Not mapped to any account");
                continue;
            }

            var categoryRules = planIdToCategoryRulesMap.GetValueOrDefault(account.PlanId, []);
            var payeeRules = planIdToPayeeRulesMap.GetValueOrDefault(account.PlanId, []);

            var statements = statementsReader.Read(fileInfo);
            var transactions = new List<YnabTransaction>(statements.Count);

            foreach (var statement in statements)
            {
                var applicableCategoryRules = categoryRules.Where(c => c.IsApplicableTo(statement.Description)).ToList();
                var categoryRule = applicableCategoryRules.OrderBy(c => c.CreatedAt).FirstOrDefault();
                if (applicableCategoryRules.Count > 1)
                {
                    output.WriteLines(
                        "More than one category rule is applicable:",
                        $"  - memo: {statement.Description}",
                        $"  - applicable rule ids: {string.Join(", ", applicableCategoryRules.Select(p => p.Id))}",
                        $"  - rule id applied: {categoryRule!.Id}");
                }

                var applicablePayeeRules = payeeRules.Where(p => p.IsApplicableTo(statement.Description)).ToList();
                var payeeRule = applicablePayeeRules.OrderBy(p => p.CreatedAt).FirstOrDefault();
                if (applicablePayeeRules.Count > 1)
                {
                    output.WriteLines(
                        "More than one payee rule is applicable:",
                        $"  - memo: {statement.Description}",
                        $"  - applicable rule ids: {string.Join(", ", applicablePayeeRules.Select(p => p.Id))}",
                        $"  - rule id applied: {payeeRule!.Id}");
                }

                var transaction = new YnabTransaction(
                    ImportId: $"{statement.DateTime:yyyy-MM-dd}{statement.DateTime:t}{statement.CardAmount}{statement.Balance}",
                    account.YnabId,
                    DateOnly.FromDateTime(statement.DateTime),
                    (int)(statement.CardAmount * YnabDecimalMultiplier),
                    categoryRule?.YnabId,
                    payeeRule?.YnabId,
                    statement.Description);
                transactions.Add(transaction);
            }

            (int createdCount, int duplicatesCount) = await ynabClient.SaveTransactionsAsync(account.Plan.YnabId, account.Plan.Token, transactions, cancellationToken);

            results.Add(new ResultModel(fileInfo.Name, transactions.Count, createdCount, duplicatesCount));
        }

        output.WriteLine(results.ToTable(headless: false));
    }

    private sealed record ResultModel(
        [property: DisplayName("File Name")] string FileName,
        [property: DisplayName("Statements Count")] int StatementsCounts,
        [property: DisplayName("Created Count")] int CreatedCount,
        [property: DisplayName("Duplicates Count")] int DuplicatesCount);
}