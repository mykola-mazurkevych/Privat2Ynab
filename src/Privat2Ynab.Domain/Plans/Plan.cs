namespace Privat2Ynab.Domain.Plans;

public sealed class Plan :
    IEntity
{
    private Plan()
    {
    }

    public int Id { get; private set; }

    public Guid YnabId { get; private set; }
    public string Name { get; private set; } = null!;

    public string Token { get; private set; } = null!;

    public static Plan Create(
        Guid ynabId,
        string name,
        string token) =>
        new()
        {
            YnabId = ynabId,
            Name = name,
            Token = token,
        };
}