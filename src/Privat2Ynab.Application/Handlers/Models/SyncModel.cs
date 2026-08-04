using Privat2Ynab.Application.Attributes;

namespace Privat2Ynab.Application.Handlers.Models;

internal sealed record SyncModel(
    [property: DisplayName("Plan Id")] int PlanId,
    [property: DisplayName("Plan Name")] string PlanName,
    int Updated,
    int Deleted);