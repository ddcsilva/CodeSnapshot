namespace CodeSnapshot.Services;

public interface IProjectExporter
{
    Task<string> ExportAsync(string projectPath, string outputPath, Action<double> reportProgress, Action<string> log, CancellationToken cancellationToken);
}