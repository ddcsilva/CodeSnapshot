namespace CodeSnapshot.Services;

public static class ProjectExporterFactory
{
    public static IProjectExporter GetExporter(string projectType)
    {
        return projectType switch
        {
            ".NET" => new DotnetProjectExporter(),
            "Angular" => throw new NotImplementedException("Angular export not yet implemented."),
            _ => throw new NotSupportedException($"Unknown project type: {projectType}")
        };
    }
}
