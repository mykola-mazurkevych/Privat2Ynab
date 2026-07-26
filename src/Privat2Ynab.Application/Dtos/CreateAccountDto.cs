namespace Privat2Ynab.Application.Dtos;

public sealed record CreateAccountDto(
    string PersonalAccessToken,
    Guid BudgetId,
    Guid AccountId,
    string FileName);