using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Outer.CLI.Docker;
using Outer.CLI.Docker.Scan;
using Outer.CLI.Docker.Scan.DockerScanFile;
using Outer.CLI.Domain;
using Outer.CLI.Jwt;
using Outer.CLI.Jwt.Decode;
using Outer.CLI.Services;
using Outer.CLI.Services.ContainerRegistryServices;
using Outer.CLI.Shared;

namespace Outer.CLI;

internal static class Program
{
    async static Task<int> Main(string[] args)
    {
        // Configure dependency injection
        var services = new ServiceCollection();
        services.AddSingleton<RootCommandCli>();
        
        // Register HTTP client and repository services
        services.AddHttpClient<DockerHubRepository>();
        services.AddHttpClient<GithubContainerRegistryRepository>();
        services.AddHttpClient<MicrosoftContainerRegistryRepository>();
        services.AddTransient<ContainerRegistryRepositoryFactory>();
        services.AddTransient<ITagSearcherService, TagSearcherService>();
        
        services.AddTransient<IReaderFile, FileReaderService>();
        services.AddTransient<IDockerfileParser, DockerfileParserService>();
        
        services.AddKeyedSingleton<ICommandHandler, JwtCommand>(JwtCommand.Enum.Jwt);
        services.AddKeyedSingleton<ICommandHandler, DecodePayloadCommand>(DecodePayloadCommand.Enum.Decode);
        
        services.AddKeyedSingleton<ICommandHandler, DockerCommand>(DockerCommand.Enum.Docker);
        services.AddKeyedSingleton<ICommandHandler, DockerScanCommand>(DockerCommand.Enum.DockerScan);  
        services.AddKeyedSingleton<ICommandHandler, DockerScanFile>(DockerCommand.Enum.DockerScanFile);
        
        
        var serviceProvider = services.BuildServiceProvider();
        RootCommand rootCommand = serviceProvider.GetRequiredService<RootCommandCli>();
        
        serviceProvider.GetKeyedService<ICommandHandler>(JwtCommand.Enum.Jwt);
        // serviceProvider.GetKeyedService<ICommandHandler>(DockerCommand.Enum.Docker);
        // 
        //
        // Option<FileInfo> fileOption = new("--dockerfile")
        // {
        //     Description = "The file to read and display on the console."
        // };
        // Option<DirectoryInfo> pathOption = new("--path")
        // {
        //     Description = "The path with group of DockerFile"
        // };
        // Option<int> maxConcurrencyOption = new("--max-concurrency")
        // {
        //     Description = "Maximum number of concurrent file processing operations (default: 4)",
        //     DefaultValueFactory = _ => 4
        // };
        // Option<string> imageOption = new("--image")
        // {
        //     Description = "Container image to search for feature/bugfix tags (e.g., nginx, microsoft/dotnet, ghcr.io/owner/repo)"
        // };
        //
        // Command readCommand = new("read", "Read and display the file.")
        // {
        //     fileOption,
        // };
        //
        // Command pathCommand = new("read-path", "Read and displays a DockerFIle group")
        // {
        //     pathOption,
        //     maxConcurrencyOption
        // };
        //
        // Command searchTagsCommand = new("search-tags", "Search for latest feature/bugfix tags in container registries")
        // {
        //     imageOption
        // };
        //
        // rootCommand.Subcommands.Add(readCommand);
        // rootCommand.Subcommands.Add(pathCommand);
        // rootCommand.Subcommands.Add(searchTagsCommand);
        //
        //
        //
        // readCommand.SetAction(async parseResult =>
        // {
        //     FileInfo parsedFile = parseResult.GetValue(fileOption) ?? throw new InvalidOperationException();
        //     var dockerFileReader = serviceProvider.GetRequiredService<IDockerfileParser>();
        //
        //     ContainerRegistry? container = await dockerFileReader.ParseDockerfileAsync(parsedFile.FullName);
        //
        //     if (container == null)
        //     {
        //         throw new InvalidOperationException($"not found image information in {parsedFile.FullName}");
        //     }
        //
        //     Console.WriteLine(container);
        //     Console.WriteLine(container.FullName);
        // });
        //
        // pathCommand.SetAction(async parseResult =>
        // {
        //     DirectoryInfo parsedPath = parseResult.GetValue(pathOption) ?? throw new InvalidOperationException();
        //     int maxConcurrency = parseResult.GetValue(maxConcurrencyOption);
        //     var dockerFileReader = serviceProvider.GetRequiredService<IDockerfileParser>();
        //     var tagSearcher = serviceProvider.GetRequiredService<ITagSearcherService>();
        //
        //     if (!parsedPath.Exists)
        //     {
        //         throw new DirectoryNotFoundException($"Directory not found: {parsedPath.FullName}");
        //     }
        //
        //     // Find all Dockerfile files asynchronously
        //     var dockerfiles = await FindDockerfilesAsync(parsedPath.FullName);
        //
        //     if (!dockerfiles.Any())
        //     {
        //         Console.WriteLine($"No Dockerfiles found in directory: {parsedPath.FullName}");
        //         return;
        //     }
        //
        //     Console.WriteLine($"Found {dockerfiles.Count} Dockerfile(s) in {parsedPath.FullName}");
        //     Console.WriteLine(new string('-', 50));
        //
        //     // Process files concurrently with controlled parallelism
        //     var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        //     var tasks = dockerfiles.Select(dockerfilePath => ProcessDockerfileAsync(
        //         dockerfilePath, dockerFileReader, semaphore));
        //
        //     var results = await Task.WhenAll(tasks);
        //
        //     Console.WriteLine(new string('-', 50));
        //     Console.WriteLine($"Processed {dockerfiles.Count} Dockerfile(s)");
        //     Console.WriteLine($"Successful: {results.Count(r => r.Success)}");
        //     Console.WriteLine($"Failed: {results.Count(r => !r.Success)}");
        //     
        //     foreach (var result in results)
        //     {
        //         if (result.Success)
        //         {
        //             Console.WriteLine($"  ✅ {result.FilePath} - Base Image: {result.Container?.FullName}");
        //             
        //             // Search for feature/bugfix tags for the found base image
        //             if (result.Container != null)
        //             {
        //                 var latestTag = await tagSearcher.SearchLatestTagAsync(result.Container.FullName);
        //                 if (latestTag != null)
        //                 {
        //                     Console.WriteLine($"    Latest feature/bugfix tag: {latestTag.FullName}");
        //                 }
        //                 else
        //                 {
        //                     Console.WriteLine("    ❌ No feature/bugfix tags found");
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             Console.WriteLine($"  ❌ {result.FilePath} - Error: {result.Error}");
        //         }
        //     }
        // });
        //
        // searchTagsCommand.SetAction(async parseResult =>
        // {
        //     string imageName = parseResult.GetValue(imageOption) ?? throw new InvalidOperationException("Image name is required");
        //     var tagSearcher = serviceProvider.GetRequiredService<ITagSearcherService>();
        //
        //     Console.WriteLine($"Searching for feature/bugfix tags for image: {imageName}");
        //     Console.WriteLine("Please wait...");
        //
        //     // Search for the latest feature/bugfix tag in container registries
        //     var latestTag = await tagSearcher.SearchLatestTagAsync(imageName);
        //
        //     if (latestTag == null)
        //     {
        //         Console.WriteLine($"❌ No feature/bugfix tags found for image: {imageName}");
        //         Console.WriteLine("Make sure the image exists and has tags containing 'feature' or 'bugfix'");
        //         return;
        //     }
        //
        //     Console.WriteLine($"✅ Latest feature/bugfix tag found:");
        //     Console.WriteLine($"   Original: {imageName}");
        //     Console.WriteLine($"   Updated:  {latestTag.FullName}");
        // });

        return await rootCommand.Parse(args).InvokeAsync();
    }

    private async static Task<List<string>> FindDockerfilesAsync(string directoryPath)
    {
        return await Task.Run(() =>
        {
            var dockerfilePatterns = new[] { "Dockerfile", "dockerfile", "*.dockerfile", "Dockerfile.*" };
            var dockerfiles = new List<string>();

            foreach (string pattern in dockerfilePatterns)
            {
                dockerfiles.AddRange(Directory.GetFiles(directoryPath, pattern, SearchOption.AllDirectories));
            }

            return dockerfiles.Distinct().ToList();
        });
    }

    private async static Task<ProcessResult> ProcessDockerfileAsync(
        string dockerfilePath, 
        IDockerfileParser dockerFileReader, 
        SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            Console.WriteLine($"\nProcessing: {dockerfilePath}");

            ContainerRegistry? container = await dockerFileReader.ParseDockerfileAsync(dockerfilePath);

            if (container == null)
            {
                Console.WriteLine($"  ⚠️  No base image found in {dockerfilePath}");
                return new ProcessResult { Success = false, FilePath = dockerfilePath };
            }
            else
            {
                Console.WriteLine($"  ✅ Base Image: {container.FullName}");
                return new ProcessResult { Success = true, FilePath = dockerfilePath, Container = container };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ❌ Error processing {dockerfilePath}: {ex.Message}");
            return new ProcessResult { Success = false, FilePath = dockerfilePath, Error = ex.Message };
        }
        finally
        {
            semaphore.Release();
        }
    }

    private record ProcessResult
    {
        public bool Success { get; init; }
        public string FilePath { get; init; } = string.Empty;
        public ContainerRegistry? Container { get; init; }
        public string? Error { get; init; }
    }
}