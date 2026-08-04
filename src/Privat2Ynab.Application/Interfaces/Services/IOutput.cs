namespace Privat2Ynab.Application.Interfaces.Services;

public interface IOutput
{
    void WriteLine(string message);
    void WriteLines(params string[] messages);
}