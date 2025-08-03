using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Outer.CLI.Domain;

namespace Outer.CLI.Docker.Scan;

public class DockerScanCommand: ICommandHandler
{
    public DockerScanCommand(
        [FromKeyedServices(DockerCommand.Enum.DockerScanFile)] ICommandHandler dockerScanFileCommand)
    {
        Command.Subcommands.Add(dockerScanFileCommand.Command);
        dockerScanFileCommand.Command.SetAction(dockerScanFileCommand.Handle);
        // Command.Subcommands.Add(dockerScanPathCommand.Command);
        // dockerScanPathCommand.Command.SetAction(dockerScanPathCommand.Handle);
        
    }
    public Command? Command { get; } = new("scan", "Scan Docker images for vulnerabilities")
    {
        DockerScanOptions.DockerFile,
        // DockerScanOptions.Path
    };
    public Task Handle(ParseResult parseResult)
    {
        throw new NotImplementedException();
    }
}