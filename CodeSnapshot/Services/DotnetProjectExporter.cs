using System.Text;

namespace CodeSnapshot.Services;

public class DotnetProjectExporter : IProjectExporter
{
    private readonly string[] _validExtensions = [".cs", ".cshtml", ".json", ".config"];
    private readonly string[] _ignoredFolders = ["bin", "obj", ".vs", ".git", ".vscode"];

    public async Task<string> ExportAsync(string projectPath, string outputPath, Action<double> reportProgress, Action<string> log)
    {
        string projectName = Path.GetFileName(projectPath);
        string date = DateTime.Now.ToString("dd-MM-yyyy");
        string outputFile = Path.Combine(outputPath, $"snapshot-{projectName}-{date}.txt");

        if (File.Exists(outputFile))
            File.Delete(outputFile);

        var files = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories)
            .Where(f =>
                _validExtensions.Contains(Path.GetExtension(f)) &&
                !_ignoredFolders.Any(ign => f.Contains(Path.DirectorySeparatorChar + ign + Path.DirectorySeparatorChar)))
            .ToList();

        using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);

        // Header
        await writer.WriteLineAsync("## Snapshot Report");
        await writer.WriteLineAsync($"Project: {projectName}");
        await writer.WriteLineAsync($"Date: {DateTime.Now:dd/MM/yyyy}");
        await writer.WriteLineAsync($"Files: {files.Count}");
        await writer.WriteLineAsync();

        WriteTreeStructure(writer, projectPath, 0);

        int total = files.Count;
        int count = 0;

        foreach (var file in files)
        {
            count++;
            reportProgress((double)count / total);
            log($"{count * 100 / total}% - {Path.GetFileName(file)}");

            string relativePath = Path.GetRelativePath(projectPath, file);
            await writer.WriteLineAsync("========================");
            await writer.WriteLineAsync($"File: {relativePath}");
            await writer.WriteLineAsync("------------------------");

            var lines = await File.ReadAllLinesAsync(file);
            foreach (var line in lines)
                await writer.WriteLineAsync(line);

            await writer.WriteLineAsync();
        }

        return outputFile;
    }

    private void WriteTreeStructure(StreamWriter writer, string directory, int level)
    {
        foreach (var dir in Directory.GetDirectories(directory))
        {
            var folderName = Path.GetFileName(dir);
            if (_ignoredFolders.Contains(folderName)) continue;

            string prefix = new string(' ', level * 4);
            writer.WriteLine($"{prefix}{folderName}");

            WriteTreeStructure(writer, dir, level + 1);
        }
    }
}
