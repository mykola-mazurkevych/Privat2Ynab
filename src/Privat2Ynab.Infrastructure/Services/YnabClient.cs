// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable NotAccessedPositionalProperty.Local

using System.Net;
using System.Text.Json.Serialization;

using Flurl;

using Privat2Ynab.Application.Dtos.Ynab;
using Privat2Ynab.Application.Interfaces.Services;

using Flurl.Http;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace Privat2Ynab.Infrastructure.Services;

internal sealed class YnabClient :
    IYnabClient
{
    public Task<YnabPlan?> GetPlanAsync(Guid planId, string token, CancellationToken cancellationToken = default) =>
        GetAsync<PlanResponse, YnabPlan?>(
            ["plans", planId],
            token,
            dataResponse => dataResponse.Data.Plan,
            _ => null,
            cancellationToken);

    public Task<YnabAccount?> GetAccountAsync(Guid planId, Guid accountId, string token, CancellationToken cancellationToken = default) =>
        GetAsync<AccountResponse, YnabAccount?>(
            ["plans", planId, "accounts", accountId],
            token,
            dataResponse => dataResponse.Data.Account,
            _ => null,
            cancellationToken);

    public Task<IReadOnlyCollection<YnabPayee>> GetPayeesAsync(Guid planId, string token, CancellationToken cancellationToken = default) =>
        GetAsync<PayeesResponse, IReadOnlyCollection<YnabPayee>>(
            ["plans", planId, "payees"],
            token,
            dataResponse => dataResponse.Data.Payees.AsReadOnly(),
            _ => [],
            cancellationToken);

    public Task<IReadOnlyCollection<YnabCategoryGroup>> GetCategoryGroupsAsync(Guid planId, string token, CancellationToken cancellationToken = default) =>
        GetAsync<CategoyGroupsResponse, IReadOnlyCollection<YnabCategoryGroup>>(
            ["plans", planId, "categories"],
            token,
            dataResponse => dataResponse.Data.CategoryGroups.AsReadOnly(),
            _ => [],
            cancellationToken);

    private static async Task<TResponse> GetAsync<TDataResponse, TResponse>(
        object[] pathSegments,
        string token,
        Func<DataResponse<TDataResponse>, TResponse> okSelector,
        Func<IFlurlResponse, TResponse> notFoundSelector,
        CancellationToken cancellationToken)
    {
        var response = await "https://api.ynab.com/v1/"
            .AppendPathSegments(pathSegments)
            .WithOAuthBearerToken(token)
            .AllowAnyHttpStatus()
            .GetAsync(cancellationToken: cancellationToken);

        return response.StatusCode switch
        {
            (int)HttpStatusCode.OK => okSelector(await response.GetJsonAsync<DataResponse<TDataResponse>>()),
            (int)HttpStatusCode.NotFound => notFoundSelector(response),
            _ => throw await CreateExceptionAsync(response),
        };
    }

    public async Task<(int CreatedCount, int DuplicatesCount)> SaveTransactionsAsync(
        Guid planId,
        string token,
        IEnumerable<YnabTransaction> transactions,
        CancellationToken cancellationToken = default)
    {
        using var jsonContent = JsonContent.Create(new SaveTransactionsRequest(transactions));
        var response = await "https://api.ynab.com/v1/"
            .AppendPathSegments("budgets", planId, "transactions")
            .WithOAuthBearerToken(token)
            .AllowAnyHttpStatus()
            .SendAsync(HttpMethod.Post, jsonContent, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return response.StatusCode switch
        {
            (int)HttpStatusCode.Created => (await response.GetJsonAsync<DataResponse<SaveTransactionsResponse>>()).Data.ToCounts(),
            _ => throw await CreateExceptionAsync(response),
        };
    }

    private static async Task<YnabApiException> CreateExceptionAsync(IFlurlResponse response) =>
        new(response.StatusCode,
            await response.GetStringAsync(),
            response.StatusCode switch
            {
                (int)HttpStatusCode.Unauthorized => "YNAB authentication failed",
                (int)HttpStatusCode.Forbidden => "YNAB authorization failed",
                (int)HttpStatusCode.BadRequest => "YNAB rejected the request",
                (int)HttpStatusCode.UnprocessableEntity => "YNAB could not process the request",
                (int)HttpStatusCode.TooManyRequests => "YNAB rate limit exceeded",
                _ => "Unexpected YNAB API response",
            });

    private sealed record DataResponse<TData>([property: JsonPropertyName("data")] TData Data);

    private sealed record AccountResponse([property: JsonPropertyName("account")] YnabAccount Account);
    private sealed record CategoyGroupsResponse([property: JsonPropertyName("category_groups")] Collection<YnabCategoryGroup> CategoryGroups);
    private sealed record PayeesResponse([property: JsonPropertyName("payees")] Collection<YnabPayee> Payees);
    private sealed record PlanResponse([property: JsonPropertyName("plan")] YnabPlan Plan);

    private sealed record SaveTransactionsRequest([property: JsonPropertyName("transactions")] IEnumerable<YnabTransaction> Transactions);
    private sealed record SaveTransactionsResponse(
        [property: JsonPropertyName("transaction_ids")] IEnumerable<string> TransactionIds,
        [property: JsonPropertyName("duplicate_import_ids")] IEnumerable<string> DuplicateImportIds)
    {
        public (int CreatedCount, int DuplicatesCount) ToCounts() =>
            new(TransactionIds.Count(), DuplicateImportIds.Count());
    }

}