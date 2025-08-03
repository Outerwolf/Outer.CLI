using System.Text.Json;
using Outer.CLI.Domain;

namespace Outer.CLI.Services.ContainerRegistryServices;

public class DockerHubRepository: IContainerRegistryRepository
{
    private readonly HttpClient _httpClient;

    public DockerHubRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ContainerRegistry?> GetLatestTags(ContainerRegistry currentRegistry)
    {
        try
        {
            // Docker Hub API endpoint for tags
            var url = $"https://registry.hub.docker.com/v2/repositories/{currentRegistry.Namespace}/{currentRegistry.Repository}/tags/?page_size=100";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsonContent = await response.Content.ReadAsStringAsync();
            var tagsResponse = JsonSerializer.Deserialize<DockerHubTagsResponse>(jsonContent);

            if (tagsResponse?.Results == null)
                return null;

            // Filter tags containing "feature" or "bugfix" and get the latest one
            var filteredTags = tagsResponse.Results
                .Where(tag => tag.Name.Contains("feature", StringComparison.OrdinalIgnoreCase) ||
                             tag.Name.Contains("bugfix", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(tag => tag.LastUpdated)
                .FirstOrDefault();

            if (filteredTags == null)
                return null;

            // Create a new ContainerRegistry with the latest feature/bugfix tag
            return ContainerRegistry.Create($"{currentRegistry.Namespace}/{currentRegistry.Repository}:{filteredTags.Name}");
        }
        catch
        {
            return null;
        }
    }

    private record DockerHubTagsResponse(DockerHubTag[]? Results);
    private record DockerHubTag(string Name, DateTime LastUpdated);
}