using Privat2Ynab.Domain.Plans;

namespace Privat2Ynab.Domain.Accounts;

public sealed class Account :
    IEntity
{
    private Account()
    {
    }

    private Account(DateTimeOffset createdAt, int planId, Guid ynabId, string name, string fileName) =>
        (CreatedAt, PlanId, YnabId, Name, FileName) = (createdAt, planId, ynabId, name, fileName);

    public int Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public int PlanId { get; private set; }
    public Plan Plan { get; private set; } = null!;

    public Guid YnabId { get; private set; }
    public string Name { get; private set; } = null!;

    public string FileName { get; private set; } = null!; // TODO: think about moving to a separate entity so Account will have a collection of files

    public static Account Create(
        DateTimeOffset createdAt,
        int planId,
        Guid ynabId,
        string name,
        string fileName) =>
        new(createdAt, planId, ynabId, name, fileName);
}