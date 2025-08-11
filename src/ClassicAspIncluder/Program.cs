using ClassicAspIncluder;

Console.WriteLine("Enter application root folder path");
string? rootFolderPath = Console.ReadLine();

if(string.IsNullOrEmpty(rootFolderPath))
{
    Console.WriteLine("Invalid root folder path");
    Console.ReadLine();
    return;
}

if(!Directory.Exists(rootFolderPath))
{
    Console.WriteLine("Folder does not exist");
    Console.ReadLine();
    return;
}

Console.WriteLine("Enter relative file path");
string? relativeFilePath = Console.ReadLine();

if(string.IsNullOrEmpty(relativeFilePath))
{
    Console.WriteLine("Invalid relative file path");
    Console.ReadLine();
    return;
}

string? fullPath = Path.Combine(rootFolderPath, relativeFilePath);
Console.WriteLine(fullPath);
Console.WriteLine();


List<string> completeFile = new FileParser(rootFolderPath).ParseFile(fullPath);



string? fileName = Path.GetFileNameWithoutExtension(relativeFilePath);
string? fileExt = Path.GetExtension(relativeFilePath);
string? directoryName = Path.GetDirectoryName(fullPath);
string? newFileName = $"{fileName}.tmp{fileExt}";
string newFilePath = Path.Combine(directoryName, newFileName);

File.WriteAllLines(newFilePath, completeFile);