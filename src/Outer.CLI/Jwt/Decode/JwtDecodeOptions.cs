using System.CommandLine;

namespace Outer.CLI.Jwt.Decode;

public static class JwtDecodeOptions
{
    public static class TokenOptions
    {
        public static readonly Option<string> GetPayloadOption = new("--token", "-t")
        {
            Description = "The JWT token to decode.",
            Required = true
        };
    }
}