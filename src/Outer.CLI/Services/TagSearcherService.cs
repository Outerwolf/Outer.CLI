using Outer.CLI.Domain;
using Outer.CLI.Services.ContainerRegistryServices;

namespace Outer.CLI.Services;

public interface ITagSearcherService
{
    Task<ContainerRegistry?> SearchLatestTagAsync(string imageName);
}

public class TagSearcherService : ITagSearcherService
{
    private readonly ContainerRegistryRepositoryFactory _repositoryFactory;

    public TagSearcherService(ContainerRegistryRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
    }

    public async Task<ContainerRegistry?> SearchLatestTagAsync(string imageName)
    {
        // Parse the image name into a ContainerRegistry object
        var containerRegistry = ContainerRegistry.Create(imageName);
        if (containerRegistry == null)
        {
            return null;
        }

        // Get the appropriate repository based on the hostname
        var repository = _repositoryFactory.GetRepository(containerRegistry);
        if (repository == null)
        {
            return null;
        }

        // Search for the latest feature/bugfix tag
        return await repository.GetLatestTags(containerRegistry);
    }
}
