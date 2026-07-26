using Privat2Ynab.Application.Interfaces.Services;

namespace Privat2Ynab.Infrastructure.Services;

internal sealed class ConsoleWriter :
    IOutputWriter
{
    public void Write(string data) =>
        Console.WriteLine(data);
}