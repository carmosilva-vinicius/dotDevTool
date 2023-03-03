using dotDevTool.src.Commands;

namespace DotDevTool
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string projectName = args[0];
            
            ProjectCreator projectCreator = new ProjectCreator(projectName);
            DependencyManager dependencyManager = new DependencyManager();
            DockerConfiguration dockerConfiguration = new DockerConfiguration(projectName);

            await projectCreator.createAsync();
            await dependencyManager.addPackagesAsync();
            dependencyManager.includeDbConnection();
            dockerConfiguration.createDockerFile();
            dockerConfiguration.createDockerCompose();

            await projectCreator.runProject();
        }

    }
}