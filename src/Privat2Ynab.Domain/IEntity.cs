namespace Privat2Ynab.Domain;

public interface IEntity
{
    int Id { get; }
    DateTimeOffset CreatedAt { get; }
}