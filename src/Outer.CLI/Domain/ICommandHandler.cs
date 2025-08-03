using System.CommandLine;

namespace Outer.CLI.Domain;

public interface ICommandHandler
{
    Command? Command { get; }
    Task Handle(ParseResult parseResult);
}