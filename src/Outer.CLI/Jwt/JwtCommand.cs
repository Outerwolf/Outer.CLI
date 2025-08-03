using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Outer.CLI.Domain;
using Outer.CLI.Jwt.Decode;
using Outer.CLI.Shared;

namespace Outer.CLI.Jwt;

public class JwtCommand: ICommandHandler
{
    public JwtCommand(RootCommandCli rootCommandCli, [FromKeyedServices(DecodePayloadCommand.Enum.Decode)] ICommandHandler commandHandler)
    {
        rootCommandCli.Add(Command);
        Command.Subcommands.Add(commandHandler.Command);
        commandHandler.Command.SetAction(commandHandler.Handle);
    }
    public Command? Command { get; } = new("jwt", "Take action to decode a JWT token payload")
    {
        JwtDecodeOptions.TokenOptions.GetPayloadOption,
    };
    public Task Handle(ParseResult parseResult)
    {
        throw new NotImplementedException();
    }
    public enum Enum
    {
        Jwt
    }
}