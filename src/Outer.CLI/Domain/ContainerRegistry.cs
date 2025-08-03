namespace Outer.CLI.Domain;

public class ContainerRegistry
{
    private const string DefaultHostName = "docker.io";
    private const string DefaultNamespace = "library";
    public string HostName { get; private set; }
    public string Namespace { get; private set; }
    public string Repository { get; private set; }
    public string Tag { get; private set; }
    
    public string FullName => $"{HostName}/{Namespace}/{Repository}:{Tag}".TrimEnd('/');

    private ContainerRegistry(string hostName = DefaultHostName, string @namespace = DefaultNamespace, string repository = "", string tag = "latest")
    {
        HostName = hostName;
        Namespace = @namespace;
        Repository = repository;
        Tag = tag;
    }
    
    public static ContainerRegistry? Create(string imageReference)
    {
        if (string.IsNullOrWhiteSpace(imageReference))
            return null;
        
        // Handle special case for "scratch"
        if (imageReference.Equals("scratch", StringComparison.OrdinalIgnoreCase))
            return null;
        
        // Split by tag first (if exists)
        var parts = imageReference.Split(':', 2);
        var imagePart = parts[0];
        var tag = parts.Length > 1 ? parts[1] : "latest";
        // Parse the image part: [hostname[:port]/][namespace/]repository
        var imageParts = imagePart.Split('/');
        string hostName = DefaultHostName;
        string @namespace = DefaultNamespace;
        string repository;

        try
        {

            if (imageParts.Length == 1)
            {
                // Just repository name (e.g., "ubuntu")
                repository = imageParts[0];
            }
            else if (imageParts.Length == 2)
            {
                // Check if first part contains a dot or port (indicating hostname)
                if (imageParts[0].Contains('.') || imageParts[0].Contains(':'))
                {
                    // hostname/repository (e.g., "myregistry.com/myapp")
                    hostName = imageParts[0];
                    @namespace = string.Empty;
                    repository = imageParts[1];
                }
                else
                {
                    // namespace/repository (e.g., "microsoft/dotnet")
                    hostName = DefaultHostName;
                    @namespace = imageParts[0];
                    repository = imageParts[1];
                }
            }
            else if (imageParts.Length >= 3)
            {
                // hostname/namespace/repository (e.g., "myregistry.com/myorg/myapp")
                hostName = imageParts[0];
                @namespace = string.Join("/", imageParts.Skip(1).Take(imageParts.Length - 2));
                repository = imageParts[^1];
            }
            else
            {
                return null; 
            }
            
            return new ContainerRegistry(hostName, @namespace, repository, tag);
        }
        catch
        {
            return null;
        }
        
    }
}