using System.CommandLine;

using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application.Dtos;
using Privat2Ynab.Application.Interfaces.Handlers;

namespace Privat2Ynab.Console.Commands;

internal static class AccountCommands
{
    extension(RootCommand rootCommand)
    {
        public RootCommand AddAccountCommands(IServiceProvider serviceProvider)
        {
            var accountHandler = serviceProvider.GetRequiredService<IAccountHandler>();

            rootCommand.Add(
                new Command("accounts")
                {
                    CreateListAccountsCommand(accountHandler),
                    CreateAddAccountCommand(accountHandler),
                    ////CreateUpdateAccountCommand(), // TODO: Decide if needed
                    CreateDeleteAccountCommand(accountHandler),
                });

            return rootCommand;
        }
    }

    private static Command CreateListAccountsCommand(IAccountHandler accountHandler)
    {
        var listAccountsCommand = new Command("list", "List accounts");
        listAccountsCommand.SetAction((_, cancellationToken) => accountHandler.ListAsync(cancellationToken));

        return listAccountsCommand;
    }

    private static Command CreateAddAccountCommand(IAccountHandler accountHandler)
    {
        var fileNameOption = new Option<string>("--file-name")
        {
            Required = true,
        };
        var ynabAccountIdOption = new Option<Guid>("--ynab-account-id")
        {
            Required = true,
        };

        var addAccountCommand = new Command("add", "Add new account")
        {
            fileNameOption,
            ynabAccountIdOption,
        };

        addAccountCommand.SetAction((parseResult, cancellationToken) =>
            accountHandler.AddAsync(
                new CreateAccountDto(
                    parseResult.GetRequiredValue(fileNameOption),
                    parseResult.GetRequiredValue(ynabAccountIdOption)),
                cancellationToken));

        return addAccountCommand;
    }

    private static Command CreateDeleteAccountCommand(IAccountHandler accountHandler)
    {
        var idOption = new Option<int>("--id")
        {
            Required = true,
        };

        var deleteAccountCommand = new Command("delete", "Delete existing account")
        {
            idOption,
        };

        deleteAccountCommand.SetAction((parseResult, cancellationToken) =>
            accountHandler.DeleteAsync(
                parseResult.GetRequiredValue(idOption),
                cancellationToken));

        return deleteAccountCommand;
    }
}