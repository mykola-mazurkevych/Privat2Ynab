namespace Privat2Ynab.Application.Interfaces.Handlers;

public interface IStatementsHandler
{
    Task HandleAsync(CancellationToken cancellationToken = default);
}