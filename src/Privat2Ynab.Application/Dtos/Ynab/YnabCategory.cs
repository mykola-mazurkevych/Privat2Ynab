using System.Text.Json.Serialization;

namespace Privat2Ynab.Application.Dtos.Ynab;

public sealed record YnabCategory(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name);