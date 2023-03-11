using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotDevTool.src.Models;
using dotDevTool.src.Models.types;

namespace dotDevTool.src.Services
{
    public class DockerService
    {
        private ProjectConfig projectConfig;


        public DockerService(ProjectConfig projectConfig)
        {
            this.projectConfig = projectConfig;
        }

        public void createDockerFile()
        {
            File.WriteAllText("Dockerfile", @$"
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . .
WORKDIR ""/src""
RUN dotnet build ""{projectConfig.ProjectName}.csproj"" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ""{projectConfig.ProjectName}.csproj"" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [""dotnet"", ""{projectConfig.ProjectName}.dll""]
");
        }

        public void createDockerCompose()
        {
            File.WriteAllText("docker-compose.yml", @$"
version: '3.9'
services:
  {getDatabaseCompose(projectConfig.Database)}
  webapi:
    build: .
    ports:
      - 80:80
      - 443:443
    environment:
      ConnectionStrings: ""Host=database; Database={projectConfig.ProjectName}; Username=dbuser; Password=dbpassword""
    {(projectConfig.Database != null ? @"depends_on:
      - database" : "")}
    restart: unless-stopped
    networks:
      - {projectConfig.ProjectName}-network
networks:
  {projectConfig.ProjectName}-network:
    driver: bridge
");
        }

        private string getDatabaseCompose(Database? database)
        {
            switch (database)
            {
                case Database.POSTGRESQL:
                    return @$"database:
    image: postgres
    environment:
      POSTGRES_USER: dbuser
      POSTGRES_PASSWORD: dbpassword
      POSTGRES_DB: ""{projectConfig.ProjectName}""
    restart: unless-stopped
    networks:
      - {projectConfig.ProjectName}-network
    volumes:
      - C:\dados\volumes\databases\POSTGRES\{projectConfig.ProjectName}:/var/lib/postgresql/data";
      case Database.MYSQL:
                    return @$"database:
    image: mysql
    ports:
      - 3306:3306
    environment:
      MYSQL_USER: dbuser
      MYSQL_PASSWORD: dbpassword
      MYSQL_ROOT_PASSWORD: dbrootpass
    volumes:
      - C:\dados\volumes\databases\MYSQL\{projectConfig.ProjectName}:/var/lib/mysql";
      case Database.MONGO: return @$"database:
    image: mongo
    ports:
      - 27017:27017
    environment:
      MONGO_INITDB_ROOT_USERNAME: dbuser
      MONGO_INITDB_ROOT_PASSWORD: dbpassword
    volumes:
      - C:\dados\volumes\databases\MYSQL\{projectConfig.ProjectName}:/var/lib/mongodb";
                default:
                    return "";
            }
        }
    
    }
}