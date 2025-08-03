using System.CommandLine;
using Outer.CLI.Domain;

namespace Outer.CLI.Docker.Scan.DockerScanPath;

public class DockerScanPathCommand: ICommandHandler
{
    public Command? Command { get; } = new("path", "Scan a Dockerfile or directory for vulnerabilities")
    {
        
    };
    public Task Handle(ParseResult parseResult)
    {
        throw new NotImplementedException();
    }
}