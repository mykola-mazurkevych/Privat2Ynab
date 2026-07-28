using Privat2Ynab.Application.Interfaces.Services;

namespace Privat2Ynab.Infrastructure.Services;

internal sealed class ConsoleOutput :
    IOutput
{
    public void Write(string message) =>
        Console.Write(message);

    public void WriteLine(string message) =>
        Console.WriteLine(message);
}