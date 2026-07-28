namespace Privat2Ynab.Domain.Plans;

public sealed class Plan :
    IEntity
{
    private Plan()
    {
    }

    private Plan(DateTimeOffset createdAt, Guid ynabId, string name, string token) =>
        (CreatedAt, YnabId, Name, Token) = (createdAt, ynabId, name, token);

    public int Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Guid YnabId { get; private set; }
    public string Name { get; private set; } = null!;

    public string Token { get; private set; } = null!;

    public static Plan Create(
        DateTimeOffset createdAt,
        Guid ynabId,
        string name,
        string token) =>
        new(createdAt, ynabId, name, token);
}