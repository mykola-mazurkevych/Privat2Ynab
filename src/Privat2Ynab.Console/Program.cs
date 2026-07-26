using System.CommandLine;

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application;
using Privat2Ynab.Console.Commands;
using Privat2Ynab.Infrastructure;

Console.WriteLine("#####################");
Console.WriteLine("#    Privat2Ynab    #");
Console.WriteLine("#####################");

await using var serviceProvider = new ServiceCollection()
    .AddInfrastructure()
    .AddApplication()
    .BuildServiceProvider();

using var cancellationTokenSource = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    // ReSharper disable once AccessToDisposedClosure
    cancellationTokenSource.Cancel();
};

await serviceProvider.GetRequiredService<IMigrator>().MigrateAsync(cancellationToken: cancellationTokenSource.Token);

return await new RootCommand()
    .AddAccountCommands(serviceProvider)
    .Parse(args)
    .InvokeAsync(cancellationToken: cancellationTokenSource.Token);