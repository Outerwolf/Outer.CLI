namespace Outer.CLI.Domain;

public interface IDockerfileParser
{
    Task<ContainerRegistry?> ParseDockerfileAsync(string dockerfilePath);
}
