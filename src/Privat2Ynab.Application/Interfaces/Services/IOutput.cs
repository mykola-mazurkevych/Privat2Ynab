namespace Privat2Ynab.Application.Interfaces.Services;

public interface IOutput
{
    void Write(string message);
    void WriteLine(string message);
}