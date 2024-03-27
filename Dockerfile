FROM mcr.microsoft.com/dotnet/aspnet:8.0.2-alpine3.18-amd64 AS base
WORKDIR /app

#build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0.201-alpine3.18-amd64 AS build
WORKDIR /src
COPY ["lrn.devgalop.dockermongo.Core", "./lrn.devgalop.dockermongo.Core"]
COPY ["lrn.devgalop.dockermongo.Infrastructure", "./lrn.devgalop.dockermongo.Infrastructure"]
COPY ["lrn.devgalop.dockermongo.Webapi", "./lrn.devgalop.dockermongo.Webapi"]
COPY ["lrn.devgalop.dockermongo.Tests", "./lrn.devgalop.dockermongo.Tests"]

COPY . .
WORKDIR "/src/."

RUN dotnet build "lrn.devgalop.dockermongo.Webapi/lrn.devgalop.dockermongo.Webapi.csproj" -c Release -o /app/build
RUN dotnet test "lrn.devgalop.dockermongo.Tests/lrn.devgalop.dockermongo.Tests.csproj"

#publish stage
FROM build AS publish
RUN dotnet publish "lrn.devgalop.dockermongo.Webapi/lrn.devgalop.dockermongo.Webapi.csproj" -c Release -o /app/publish

#exec stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "lrn.devgalop.dockermongo.Webapi.dll"]