using Privat2Ynab.Application.Dtos;

namespace Privat2Ynab.Application.Interfaces.Services;

public interface IStatementsReader
{
    IReadOnlyList<StatementDto> Read(FileInfo fileInfo);
}