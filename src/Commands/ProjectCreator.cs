using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotDevTool.src.Services;

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
            await CommandTerminal.RunCommand("dotnet", $"new webapi -n {projectName} --force");
            Directory.SetCurrentDirectory(projectName);
            // Directory.CreateDirectory("Data");
        }

        public async Task runProject(){
            await CommandTerminal.RunCommand("docker", "compose up --build -d");
        }

        
    }
}