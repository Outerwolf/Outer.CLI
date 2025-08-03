namespace Outer.CLI.Domain;

public interface IContainerRegistryRepository
{
    Task <ContainerRegistry?> GetLatestTags(ContainerRegistry currentRegistry);
}