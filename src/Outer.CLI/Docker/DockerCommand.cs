using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Outer.CLI.Domain;
using Outer.CLI.Shared;

namespace Outer.CLI.Docker;

public class DockerCommand : ICommandHandler
{
    public DockerCommand(RootCommandCli rootCommandCli, [FromKeyedServices(Enum.DockerScan)] ICommandHandler dockerScanCommand)
    {
        rootCommandCli.Add(Command);
        Command.Subcommands.Add(dockerScanCommand.Command);
    }
    public Command? Command { get; } = new("docker", "Manage Docker containers and images")
    {
    };
    public Task Handle(ParseResult parseResult)
    {
        throw new NotImplementedException();
    }
    public enum Enum
    {
        Docker,
        DockerScan,
        DockerScanFile,
        DockerScanPath
    }
}