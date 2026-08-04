using Privat2Ynab.Application.Interfaces.Services;

namespace Privat2Ynab.Infrastructure.Services;

internal sealed class ConsoleOutput :
    IOutput
{
    public void WriteLine(string message) =>
        Console.WriteLine(message);

    public void WriteLines(params string[] messages)
    {
        foreach (var message in messages)
        {
            WriteLine(message);
        }
    }
}