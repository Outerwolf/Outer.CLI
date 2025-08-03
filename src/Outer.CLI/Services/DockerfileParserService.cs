using System.Text.RegularExpressions;
using Outer.CLI.Domain;

namespace Outer.CLI.Services;

public class DockerfileParserService : IDockerfileParser
{
    private static readonly Regex FromRegex = new(@"^\s*FROM\s+(.+?)(?:\s+AS\s+\w+)?\s*$", 
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public async Task<ContainerRegistry?> ParseDockerfileAsync(string dockerfilePath)
    {
        if (!File.Exists(dockerfilePath))
        {
            throw new FileNotFoundException($"Dockerfile not found at: {dockerfilePath}");
        }

        await foreach (string line in File.ReadLinesAsync(dockerfilePath))
        {
            var trimmedLine = line.Trim();
            
            // Skip comments and empty lines
            if (string.IsNullOrWhiteSpace(trimmedLine) || trimmedLine.StartsWith('#'))
                continue;

            var match = FromRegex.Match(trimmedLine);
            if (match.Success)
            {
                string imageReference = match.Groups[1].Value.Trim();
                return ContainerRegistry.Create(imageReference);
            }
        }

        return null;
    }
}
