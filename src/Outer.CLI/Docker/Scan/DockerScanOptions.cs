using System.CommandLine;

namespace Outer.CLI.Docker.Scan;

public static class DockerScanOptions
{
    public static readonly Option<FileInfo> DockerFile = new("--dockerfile", "-d")
    {
        Description = "The path to the Dockerfile to scan.",
        Required = true
    };
    public static readonly Option<string> Path = new("--path", "-p")
    {
        Description = "The path to the directory containing Dockerfiles to scan.",
        Required = false
    };
}