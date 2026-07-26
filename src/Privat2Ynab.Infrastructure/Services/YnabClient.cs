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
    public async Task<YnabPlan?> GetPlanAsync(Guid planId, string token, CancellationToken cancellationToken = default)
    {
        var response = await "https://api.ynab.com/v1/plans"
            .AppendPathSegment(planId)
            .WithOAuthBearerToken(token)
            .AllowAnyHttpStatus()
            .GetAsync(cancellationToken: cancellationToken);

        return response.StatusCode switch
        {
            (int)HttpStatusCode.OK => (await response.GetJsonAsync<DataResponse<PlanResponse>>()).Data.Plan,
            (int)HttpStatusCode.NotFound => null,
            _ => throw new NotSupportedException($"Http status code {response.StatusCode} is not supported"),
        };
    }

    public async Task<YnabAccount?> GetAccountAsync(Guid planId, Guid accountId, string token, CancellationToken cancellationToken = default)
    {
        var response = await "https://api.ynab.com/v1/plans"
            .AppendPathSegments(planId, "accounts", accountId)
            .WithOAuthBearerToken(token)
            .AllowAnyHttpStatus()
            .GetAsync(cancellationToken: cancellationToken);

        return response.StatusCode switch
        {
            (int)HttpStatusCode.OK => (await response.GetJsonAsync<DataResponse<AccountResponse>>()).Data.Account,
            (int)HttpStatusCode.NotFound => null,
            _ => throw new NotSupportedException($"Http status code {response.StatusCode} is not supported"),
        };
    }

    public async Task<IReadOnlyList<YnabPayee>> GetPayeesAsync(Guid planId, string token, CancellationToken cancellationToken = default)
    {
        var response = await "https://api.ynab.com/v1/plans"
            .AppendPathSegments(planId, "payees")
            .WithOAuthBearerToken(token)
            .AllowAnyHttpStatus()
            .GetAsync(cancellationToken: cancellationToken);

        return response.StatusCode switch
        {
            (int)HttpStatusCode.OK => (await response.GetJsonAsync<DataResponse<PayeesResponse>>()).Data.Payees.AsReadOnly(),
            (int)HttpStatusCode.NotFound => [],
            _ => throw new NotSupportedException($"Http status code {response.StatusCode} is not supported"),
        };
    }

    public async Task<IReadOnlyCollection<YnabCategoryGroup>> GetCategoryGroupsAsync(Guid planId, string token, CancellationToken cancellationToken = default)
    {
        var response = await "https://api.ynab.com/v1/plans"
            .AppendPathSegments(planId, "categories")
            .WithOAuthBearerToken(token)
            .AllowAnyHttpStatus()
            .GetAsync(cancellationToken: cancellationToken);

        return response.StatusCode switch
        {
            (int)HttpStatusCode.OK => (await response.GetJsonAsync<DataResponse<CategoyGroupsResponse>>()).Data.CategoryGroups.AsReadOnly(),
            (int)HttpStatusCode.NotFound => [],
            _ => throw new NotSupportedException($"Http status code {response.StatusCode} is not supported"),
        };
    }

    private sealed record DataResponse<TData>([property: JsonPropertyName("data")] TData Data);

    private sealed record AccountResponse([property: JsonPropertyName("account")] YnabAccount Account);
    private sealed record CategoyGroupsResponse([property: JsonPropertyName("category_groups")] Collection<YnabCategoryGroup> CategoryGroups);
    private sealed record PayeesResponse([property: JsonPropertyName("payees")] Collection<YnabPayee> Payees);
    private sealed record PlanResponse([property: JsonPropertyName("plan")] YnabPlan Plan);
    
}