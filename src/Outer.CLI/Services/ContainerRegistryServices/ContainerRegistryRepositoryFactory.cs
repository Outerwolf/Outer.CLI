using Microsoft.Extensions.DependencyInjection;
using Outer.CLI.Domain;

namespace Outer.CLI.Services.ContainerRegistryServices;

public class ContainerRegistryRepositoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ContainerRegistryRepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IContainerRegistryRepository? GetRepository(ContainerRegistry containerRegistry)
    {
        return containerRegistry.HostName.ToLowerInvariant() switch
        {
            "docker.io" or "hub.docker.com" => _serviceProvider.GetService<DockerHubRepository>(),
            "ghcr.io" => _serviceProvider.GetService<GithubContainerRegistryRepository>(),
            "mcr.microsoft.com" => _serviceProvider.GetService<MicrosoftContainerRegistryRepository>(),
            _ => null // Unsupported registry
        };
    }
}
