using System.CommandLine;

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

using Privat2Ynab.Application;
using Privat2Ynab.Console.Commands;
using Privat2Ynab.Infrastructure;

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

Console.OutputEncoding = System.Text.Encoding.UTF8;

await new RootCommand()
    .Configure(serviceProvider)
    .Parse(args)
    .InvokeAsync(cancellationToken: cancellationTokenSource.Token);