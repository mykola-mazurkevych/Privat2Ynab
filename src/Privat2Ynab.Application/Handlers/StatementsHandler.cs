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

        var accounts = await repository.ListAsync<Account>(cancellationToken);
        var fileNameToYnabAccountMap = accounts
            .ToDictionary(
                a => a.FileName,
                StringComparer.OrdinalIgnoreCase);

        var categoryRules = await repository.ListAsync<CategoryRule>(cancellationToken);
        var payeeRules = await repository.ListAsync<PayeeRule>(cancellationToken);

        List<ResultModel> results = [];

        foreach (var fileInfo in fileInfos)
        {
            output.WriteLine($"Processing file {fileInfo.Name}...");

            if (!fileNameToYnabAccountMap.TryGetValue(fileInfo.Name, out var account))
            {
                output.WriteLine("Not mapped to any account");
                continue;
            }

            var transactions = statementsReader
                .Read(fileInfo, cancellationToken)
                .Select(statement =>
                    new YnabTransaction(
                        ImportId: $"{statement.DateTime:yyyy-MM-dd}{statement.DateTime:t}{statement.CardAmount}{statement.Balance}",
                        account.YnabId,
                        DateOnly.FromDateTime(statement.DateTime),
                        (int)(statement.CardAmount * 1000),
                        categoryRules.FirstOrDefault(categoryRule => categoryRule.IsApplicableTo(statement.Description))?.YnabId,
                        payeeRules.FirstOrDefault(payeeRule => payeeRule.IsApplicableTo(statement.Description))?.YnabId,
                        statement.Description))
                .ToList()
                .AsReadOnly();

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