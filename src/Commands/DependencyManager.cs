using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotDevTool.src.Commands
{
    public class DependencyManager
    {
        public async Task addPackagesAsync(){
            // Adiciona o pacote Npgsql.EntityFrameworkCore.PostgreSQL ao projeto
            await RunCommand("dotnet", $"add package Npgsql.EntityFrameworkCore.PostgreSQL");
        }

        public void includeDbConnection(){
            string programFileRegion = "Add services to the container.";
            string programCs = File.ReadAllText($"Program.cs");
            
            int startIndex = programCs.IndexOf(programFileRegion);

            int endIndex = startIndex + programFileRegion.Length;
            programCs = programCs.Insert(endIndex, @$"
string connectionString = builder.Configuration.GetValue<string>(""ConnectionStrings"");
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseLazyLoadingProxies().UseNpgsql(connectionString);");

            File.WriteAllText("Program.cs", programCs);
        }
        private async Task<int> RunCommand(string command, string args)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = command,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            var process = System.Diagnostics.Process.Start(psi);
            process.OutputDataReceived += (_, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (_, e) => Console.WriteLine(e.Data);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();
            return process.ExitCode;
        }
    }
}