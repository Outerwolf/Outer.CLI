using System.CommandLine;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Outer.CLI.Domain;

namespace Outer.CLI.Jwt.Decode;

public class DecodePayloadCommand: ICommandHandler
{
    public Command? Command { get; } = new("decode", "Take action to decode a JWT token payload")
    {
        JwtDecodeOptions.TokenOptions.GetPayloadOption,
    };
    public enum Enum
    {
        Decode
    }

    public Task Handle(ParseResult parseResult)
    {
        string? payload = parseResult.GetValue(JwtDecodeOptions.TokenOptions.GetPayloadOption);
        if (string.IsNullOrEmpty(payload))
        {
            Console.WriteLine("Payload is empty.");
            return Task.CompletedTask;
        }

        try
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken = jwtHandler.ReadJwtToken(payload);
            if (jwtToken == null)
            {
                Console.WriteLine("Invalid JWT token.");
                return Task.CompletedTask;
            }
            Console.WriteLine("Decoded JWT Payload:");
            Console.WriteLine("=================================");
            // print as json
            Console.WriteLine(JsonSerializer.Serialize(jwtToken.Payload, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
            Console.WriteLine("=================================");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error decoding payload: {ex.Message}");
        }

        return Task.CompletedTask;
    }

}

