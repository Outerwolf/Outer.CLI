
using Outer.CLI.Domain;

namespace Outer.CLI.Services;

public class FileReaderService : IReaderFile
{
    public async Task ReadFileAsync(string filePath)
    {
        await foreach (string line in File.ReadLinesAsync(filePath))
        {
            Console.WriteLine(line);
        }
    }
}