namespace CodeSnapshot.Services;

public static class ProjectExporterFactory
{
    public static IProjectExporter GetExporter(string projectType)
    {
        return projectType switch
        {
            "dotnet" => new DotnetProjectExporter(),
            "angular" => throw new NotImplementedException("Angular export not yet implemented."),
            _ => throw new NotSupportedException($"Unknown project type: {projectType}")
        };
    }
}
