namespace ClassicAspIncluder
{
    internal class FileParser
    {
        public FileParser(string applicationRoot)
        {
            _applicationRoot = applicationRoot;
        }

        public List<string> ParseFile(string? fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return new List<string>();

            if(!File.Exists(fullPath))
                return new List<string> { $"<% 'Unable to find path: {fullPath}  %>" };

            var fileLines = File.ReadAllLines(fullPath).ToList();

            List<string> completeFile = new List<string>();

            string previousLine = null;
            foreach (var fileLine in fileLines)
            {
                var result = ParseFileLine(fileLine);

                switch(result.Item1)
                {
                    case "fileline":
                        completeFile.Add(result.Item2);
                        break;
                    case "virtual":
                        string virtualFilePath = Path.Combine(_applicationRoot, result.Item2);
                        completeFile.Add($"<% '''''START {virtualFilePath}''''' %>");
                        completeFile.AddRange(ParseFile(virtualFilePath));
                        completeFile.Add($"<% '''''END {virtualFilePath}''''' %>");
                        break;
                    case "file":
                        string fileReferencePath = Path.Combine(Path.GetDirectoryName(fullPath), result.Item2);
                        completeFile.Add($"<% '''''START {fileReferencePath}''''' %>");
                        completeFile.AddRange(ParseFile(fileReferencePath));
                        completeFile.Add($"<% '''''END {fileReferencePath}''''' %>");
                        break;
                }
            }

            return completeFile;
        }

        private Tuple<string, string> ParseFileLine(string fileLine)
        {
            var singleLine = fileLine.Replace(" ", "");
            if (!singleLine.StartsWith("<!--#include"))
                return new Tuple<string, string>("fileline", fileLine);

            int firstQuoteIndex = fileLine.IndexOf("\"");
            int secondQuoteIndex = fileLine.IndexOf("\"", firstQuoteIndex + 1);

            bool isVirtual = fileLine.IndexOf("virtual") != -1;
            bool isFile = fileLine.IndexOf("file") != -1;

            string includeFilePath = fileLine.Substring(firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1);

            if (isVirtual)
                return new Tuple<string, string>("virtual", includeFilePath.Substring(1));

            if (isFile)
                return new Tuple<string, string>("file", includeFilePath);

            return new Tuple<string, string>("fileline", fileLine);
        }

        private string _applicationRoot;
    }
}
