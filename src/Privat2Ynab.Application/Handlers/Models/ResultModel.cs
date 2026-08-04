using Privat2Ynab.Application.Attributes;

namespace Privat2Ynab.Application.Handlers.Models;

internal sealed record ResultModel(
    [property: DisplayName("File Name")] string FileName,
    [property: DisplayName("Statements Count")] int StatementsCounts,
    [property: DisplayName("Created Count")] int CreatedCount,
    [property: DisplayName("Duplicates Count")] int DuplicatesCount);