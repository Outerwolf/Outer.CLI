using System.Text.Json;
using Outer.CLI.Domain;

namespace Outer.CLI.Services.ContainerRegistryServices;

public class GithubContainerRegistryRepository: IContainerRegistryRepository
{
    private readonly HttpClient _httpClient;

    public GithubContainerRegistryRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ContainerRegistry?> GetLatestTags(ContainerRegistry currentRegistry)
    {
        try
        {
            // GitHub Container Registry uses GitHub API for package information
            // Format: ghcr.io/owner/repo -> API: /orgs/{owner}/packages/container/{repo}/versions
            var parts = currentRegistry.Namespace.Split('/');
            var owner = parts[0];
            var packageName = currentRegistry.Repository;
            
            var url = $"https://api.github.com/orgs/{owner}/packages/container/{packageName}/versions?per_page=100";
            
            // GitHub API requires User-Agent header
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Outer-Container-Tool");
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var jsonContent = await response.Content.ReadAsStringAsync();
            var versions = JsonSerializer.Deserialize<GitHubPackageVersion[]>(jsonContent);

            if (versions == null)
                return null;

            // Filter versions with tags containing "feature" or "bugfix"
            var filteredVersion = versions
                .Where(v => v.Metadata?.Container?.Tags?.Any(tag => 
                    tag.Contains("feature", StringComparison.OrdinalIgnoreCase) ||
                    tag.Contains("bugfix", StringComparison.OrdinalIgnoreCase)) == true)
                .OrderByDescending(v => v.UpdatedAt)
                .FirstOrDefault();

            if (filteredVersion?.Metadata?.Container?.Tags == null)
                return null;

            // Get the first feature/bugfix tag
            var latestTag = filteredVersion.Metadata.Container.Tags
                .FirstOrDefault(tag => tag.Contains("feature", StringComparison.OrdinalIgnoreCase) ||
                                      tag.Contains("bugfix", StringComparison.OrdinalIgnoreCase));

            if (latestTag == null)
                return null;

            return ContainerRegistry.Create($"ghcr.io/{owner}/{packageName}:{latestTag}");
        }
        catch
        {
            return null;
        }
    }

    private record GitHubPackageVersion(DateTime UpdatedAt, GitHubMetadata? Metadata);
    private record GitHubMetadata(GitHubContainer? Container);
    private record GitHubContainer(string[]? Tags);
}