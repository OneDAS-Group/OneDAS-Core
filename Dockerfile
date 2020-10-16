# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy everything else and build app
COPY . .
WORKDIR /source/src/OneDas.DataManagement.Explorer
RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["./OneDas.DataManagement.Explorer"]