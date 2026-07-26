namespace Privat2Ynab.Application.Dtos;

public sealed record CreateAccountDto(
    int PlanId,
    Guid YnabId,
    string FileName);