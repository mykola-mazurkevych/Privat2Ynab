using System.Text.Json.Serialization;

namespace Privat2Ynab.Application.Dtos.Ynab;

public sealed record YnabAccount(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name);