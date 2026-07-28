using System.Globalization;

using ClosedXML.Excel;

using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Interfaces.Services;

namespace Privat2Ynab.Infrastructure.Services;

internal sealed class StatementsReader :
    IStatementsReader
{
    public IReadOnlyList<StatementDto> Read(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook(fileInfo.FullName);
        var worksheet = workbook.Worksheet(1);

        return worksheet.RowsUsed().Skip(2)
            .Select(row =>
                new StatementDto(
                    DateTime.ParseExact(row.Cell("A").GetString(), "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                    row.Cell("B").GetString(),
                    row.Cell("C").GetString(),
                    row.Cell("D").GetString(),
                    Convert.ToDecimal(row.Cell("E").GetDouble()),
                    row.Cell("F").GetString(),
                    Convert.ToDecimal(row.Cell("G").GetDouble()),
                    row.Cell("H").GetString(),
                    Convert.ToDecimal(row.Cell("I").GetDouble()),
                    row.Cell("J").GetString()))
            .ToList()
            .AsReadOnly();
    }
}