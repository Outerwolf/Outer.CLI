namespace Outer.CLI.Domain;

public interface IReaderFile
{
    Task ReadFileAsync(string filePath);
}