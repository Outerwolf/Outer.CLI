using System.CommandLine;
using Outer.CLI.Domain;

namespace Outer.CLI.Docker.Scan.DockerScanFile;

public class DockerScanFile : ICommandHandler
{
    private readonly IDockerfileParser _dockerfileParser;
    public Command? Command { get; } = null;

    public DockerScanFile(IDockerfileParser dockerfileParser)
    {
        _dockerfileParser = dockerfileParser;
    }


    public async Task Handle(ParseResult parseResult)
    {
        FileInfo parsedFile = parseResult.GetValue(DockerScanOptions.DockerFile) ?? throw new InvalidOperationException();

        ContainerRegistry? container = await _dockerfileParser.ParseDockerfileAsync(parsedFile.FullName);

        if (container == null)
        {
            throw new InvalidOperationException($"not found image information in {parsedFile.FullName}");
        }

        Console.WriteLine(container);
        Console.WriteLine(container.FullName);
    }
}