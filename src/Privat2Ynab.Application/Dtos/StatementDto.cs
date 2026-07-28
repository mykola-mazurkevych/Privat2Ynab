namespace Privat2Ynab.Application.Dtos;

public sealed record StatementDto(
    DateTime DateTime,
    string Category,
    string? CardNumber,
    string? Description,
    decimal CardAmount,
    string CardCurrency,
    decimal TransactionAmount,
    string TransactionCurrency,
    decimal Balance,
    string BalanceCurrency);