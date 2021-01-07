# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy everything else and build app
COPY . .
WORKDIR /source/src/OneDas.DataManagement.Explorer
ENV PATH="/root/.dotnet/tools:${PATH}" 
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN libman restore
RUN dotnet restore
RUN ls -la /root/.nuget/packages/microsoft.netcore.app.ref/5.0.0/ref/net5.0
RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["./OneDas.DataManagement.Explorer"]