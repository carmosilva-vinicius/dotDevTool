using System.CommandLine;
using dotDevTool.src.Commands;
using dotDevTool.src.Models;
using dotDevTool.src.Models.types;
using dotDevTool.src.Services;

namespace DotDevTool
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine($"Directory: {Directory.GetCurrentDirectory()}");
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

            rootCommand.SetHandler(async (name, database) =>
                {
                    projectConfig.ProjectName = name ?? "MeuProjeto";

                    if (database == null)
                    {
                        projectConfig.Database = null;
                    }
                    else
                    {
                        if (Enum.TryParse<Database>(database, true, out db))
                        {
                            projectConfig.Database = db;
                        }
                        else { Console.WriteLine($"Banco de dados não encontrado, selecione uma opção valida"); }
                    }

                    Console.WriteLine($@"Start Creating a project:
                        Nome: {projectConfig.ProjectName}
                        Database: {projectConfig.Database}
                    ");

                    ProjectCreator projectCreator = new ProjectCreator(projectConfig.ProjectName);
                    DependencyService dependencyService = new DependencyService(projectConfig.Database);
                    DockerService dockerService = new DockerService(projectConfig);

                    await projectCreator.createAsync();
                    await dependencyService.addPackagesAsync();
                    dependencyService.includeDbConnection();
                    dockerService.createDockerFile();
                    dockerService.createDockerCompose();

                    await projectCreator.runProject();
                },
                projectName, database);

            return await rootCommand.InvokeAsync(args);
        }
    }
}