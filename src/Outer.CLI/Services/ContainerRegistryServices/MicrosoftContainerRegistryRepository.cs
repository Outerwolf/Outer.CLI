using System.Text.Json;
using Outer.CLI.Domain;

namespace Outer.CLI.Services.ContainerRegistryServices;

public class MicrosoftContainerRegistryRepository: IContainerRegistryRepository
{
    private readonly HttpClient _httpClient;

    public MicrosoftContainerRegistryRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ContainerRegistry?> GetLatestTags(ContainerRegistry currentRegistry)
    {
        try
        {
            // Microsoft Container Registry (mcr.microsoft.com) uses Azure Container Registry APIs
            // For public repositories, we can use the catalog API
            var repository = $"{currentRegistry.Namespace}/{currentRegistry.Repository}";
            var url = $"https://mcr.microsoft.com/v2/{repository}/tags/list";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsonContent = await response.Content.ReadAsStringAsync();
            var tagsResponse = JsonSerializer.Deserialize<McrTagsResponse>(jsonContent);

            if (tagsResponse?.Tags == null || !tagsResponse.Tags.Any())
                return null;

            // Filter tags containing "feature" or "bugfix"
            var filteredTags = tagsResponse.Tags
                .Where(tag => tag.Contains("feature", StringComparison.OrdinalIgnoreCase) ||
                             tag.Contains("bugfix", StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!filteredTags.Any())
                return null;

            // For MCR, we can't easily get creation dates from the tags list API
            // So we'll take the first matching tag (they're usually ordered)
            var latestTag = filteredTags.First();

            return ContainerRegistry.Create($"mcr.microsoft.com/{repository}:{latestTag}");
        }
        catch
        {
            // Fallback: try alternative approach using manifest API for more detailed info
            return await GetLatestTagsWithManifestAsync(currentRegistry);
        }
    }

    private async Task<ContainerRegistry?> GetLatestTagsWithManifestAsync(ContainerRegistry currentRegistry)
    {
        try
        {
            var repository = $"{currentRegistry.Namespace}/{currentRegistry.Repository}";
            var url = $"https://mcr.microsoft.com/v2/{repository}/tags/list";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsonContent = await response.Content.ReadAsStringAsync();
            var tagsResponse = JsonSerializer.Deserialize<McrTagsResponse>(jsonContent);

            if (tagsResponse?.Tags == null)
                return null;

            // Filter and sort tags containing "feature" or "bugfix"
            var filteredTags = tagsResponse.Tags
                .Where(tag => tag.Contains("feature", StringComparison.OrdinalIgnoreCase) ||
                             tag.Contains("bugfix", StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(tag => tag) // Basic string ordering as fallback
                .FirstOrDefault();

            if (filteredTags == null)
                return null;

            return ContainerRegistry.Create($"mcr.microsoft.com/{repository}:{filteredTags}");
        }
        catch
        {
            return null;
        }
    }

    private record McrTagsResponse(string Name, string[]? Tags);
}