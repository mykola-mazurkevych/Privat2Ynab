using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Privat2Ynab.Application.Dtos.Ynab;

public sealed record YnabCategoryGroup(
    [property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("categories")] Collection<YnabCategory> Categories);