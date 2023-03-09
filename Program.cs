using System.CommandLine;
using dotDevTool.src.Commands;
using dotDevTool.src.Models;
using dotDevTool.src.Models.types;

namespace DotDevTool
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {

            ProjectConfig projectConfig = new ProjectConfig();
            Database db;

            var projectName = new Option<string?>(
                name: "--name",
                description: "Project name to be create."
            );

            var database = new Option<string?>(
                name: "--database",
                description: "Database to persist your project data."
            );

            var rootCommand = new RootCommand("Sample app for System.CommandLine");
            rootCommand.AddOption(projectName);
            rootCommand.AddOption(database);

            rootCommand.SetHandler((name, database) =>
                {   
                    projectConfig.ProjectName = name;
                    Enum.TryParse<Database>(database, out db);
                    projectConfig.Database = db;

                    Console.WriteLine($"Projeto: {projectConfig.ProjectName ?? "MeuProjeto"}");
                    Console.WriteLine($"Database: {projectConfig.Database}");
                },
                projectName, database);
            return await rootCommand.InvokeAsync(args);

            // string projectName = args[0];

            // ProjectCreator projectCreator = new ProjectCreator(projectName);
            // DependencyManager dependencyManager = new DependencyManager();
            // DockerConfiguration dockerConfiguration = new DockerConfiguration(projectName);

            // await projectCreator.createAsync();
            // await dependencyManager.addPackagesAsync();
            // dependencyManager.includeDbConnection();
            // dockerConfiguration.createDockerFile();
            // dockerConfiguration.createDockerCompose();

            // await projectCreator.runProject();
        }
    }
}