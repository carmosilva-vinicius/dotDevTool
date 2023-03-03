using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotDevTool.src.Commands
{
    public class ProjectCreator
    {
        private string projectName;

        public ProjectCreator(string projectName)
        {
            this.projectName = projectName;
        }

        public async Task createAsync(){
            await RunCommand("dotnet", $"new webapi -n {projectName} --force");
            Directory.SetCurrentDirectory(projectName);
            Directory.CreateDirectory("Data");
        }

        public async Task runProject(){
            await RunCommand("docker", "compose up --build");
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