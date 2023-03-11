using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotDevTool.src.Models.types;
using dotDevTool.src.Services;

namespace dotDevTool.src.Services
{
    public class DependencyService
    {
        private Database? database;

        public DependencyService(Database? database)
        {
            this.database = database;
        }

        public async Task addPackagesAsync()
        {
            if (database != null)
                await CommandTerminal.RunCommand("dotnet", $"add package Npgsql.EntityFrameworkCore.PostgreSQL");
        }

        public void includeDbConnection()
        {
            if (database != null)
            {
                string programFileRegion = "Add services to the container.";
                string programCs = File.ReadAllText($"Program.cs");

                int startIndex = programCs.IndexOf(programFileRegion);

                int endIndex = startIndex + programFileRegion.Length;
                programCs = programCs.Insert(endIndex, @$"
string connectionString = builder.Configuration.GetValue<string>(""ConnectionStrings"");
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseLazyLoadingProxies().UseNpgsql(connectionString);");

                File.WriteAllText("Program.cs", programCs);
            }
        }

    }
}