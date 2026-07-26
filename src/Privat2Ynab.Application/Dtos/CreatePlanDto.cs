namespace Privat2Ynab.Application.Dtos;

public sealed record CreatePlanDto(
    Guid PlanId,
    string Token);