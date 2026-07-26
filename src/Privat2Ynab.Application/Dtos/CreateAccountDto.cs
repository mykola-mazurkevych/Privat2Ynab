namespace Privat2Ynab.Application.Dtos;

public sealed record CreateAccountDto(
    string FileName,
    Guid YnabAccountId);