// ReSharper disable ClassNeverInstantiated.Local

using System.Net;
using System.Text.Json.Serialization;

using Flurl;

using Privat2Ynab.Application.Dtos.Ynab;
using Privat2Ynab.Application.Interfaces.Services;

using Flurl.Http;
using System.Collections.ObjectModel;

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
            _ => throw new NotSupportedException($"Http status code {response.StatusCode} is not supported"),
        };
    }

    private sealed record DataResponse<TData>([property: JsonPropertyName("data")] TData Data);

    private sealed record AccountResponse([property: JsonPropertyName("account")] YnabAccount Account);
    private sealed record CategoyGroupsResponse([property: JsonPropertyName("category_groups")] Collection<YnabCategoryGroup> CategoryGroups);
    private sealed record PayeesResponse([property: JsonPropertyName("payees")] Collection<YnabPayee> Payees);
    private sealed record PlanResponse([property: JsonPropertyName("plan")] YnabPlan Plan);
}