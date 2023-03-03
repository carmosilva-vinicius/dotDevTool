using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotDevTool.src.Commands
{
    public class DockerConfiguration
    {
    private string _projectName;

        public DockerConfiguration(string projectName)
        {
            _projectName = projectName;
        }

        public void createDockerFile(){
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
RUN dotnet build ""{_projectName}.csproj"" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ""{_projectName}.csproj"" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [""dotnet"", ""{_projectName}.dll""]
");
        }

        public void createDockerCompose(){
            File.WriteAllText("docker-compose.yml", @$"
version: '3.9'
services:
  database:
    image: postgres
    environment:
      POSTGRES_USER: dbuser
      POSTGRES_PASSWORD: senha
      POSTGRES_DB: ""{_projectName}""
    restart: unless-stopped
    networks:
      - {_projectName}-network
    volumes:
      - C:\dados\volumes\databases\POSTGRES\{_projectName}:/var/lib/postgresql/data
  webapi:
    build: .
    ports:
      - 80:80
      - 443:443
    environment:
      ConnectionStrings: ""Host=database; Database={_projectName}; Username=postgres; Password=senha""
    depends_on:
      - database
    restart: unless-stopped
    networks:
      - {_projectName}-network
networks:
  {_projectName}-network:
    driver: bridge
");
        }
    }
}