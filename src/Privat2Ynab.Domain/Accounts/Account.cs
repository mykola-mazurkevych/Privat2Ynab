// ReSharper disable UnusedMember.Global

using Privat2Ynab.Domain.Plans;

namespace Privat2Ynab.Domain.Accounts;

public sealed class Account :
    IEntity
{
    private Account()
    {
    }

    public int Id { get; private set; }

    public int PlanId { get; private set; }
    public Plan Plan { get; private set; } = null!;

    public Guid YnabId { get; private set; }
    public string Name { get; private set; } = null!;

    public string FileName { get; private set; } = null!; // TODO: think about moving to a separate entity so Account will have a collection of files

    public static Account Create(
        int planId,
        Guid ynabId,
        string name,
        string fileName) =>
        new()
        {
            PlanId = planId,
            YnabId = ynabId,
            Name = name,
            FileName = fileName,
        };
}