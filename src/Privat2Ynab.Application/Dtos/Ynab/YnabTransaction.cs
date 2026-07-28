using System.Text.Json.Serialization;

namespace Privat2Ynab.Application.Dtos.Ynab;

public sealed record YnabTransaction(
    ////[property: JsonPropertyName("id")] Guid Id,
    [property: JsonPropertyName("import_id")] string ImportId,
    [property: JsonPropertyName("account_id")] Guid AccountId,
    [property: JsonPropertyName("date")] DateOnly Date,
    [property: JsonPropertyName("amount")] int Amount,
    [property: JsonPropertyName("category_id")] Guid? CategoryId,
    [property: JsonPropertyName("payee_id")] Guid? PayeeId,
    [property: JsonPropertyName("memo")] string? Memo);