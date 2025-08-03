using System.CommandLine;

namespace Outer.CLI.Shared;

public class RootCommandCli: RootCommand
{
    public RootCommandCli() : base("Outer Container CLI")
    {
        Description = "Outer Container Command Line Interface";
    }
}