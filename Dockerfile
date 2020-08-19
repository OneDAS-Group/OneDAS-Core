# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# copy everything else and build app
COPY . .
WORKDIR /source/src/OneDas.DataManagement.Explorer
RUN dotnet publish -c release -o /app -r linux-musl-x64 --self-contained false

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["./OneDas.DataManagement.Explorer"]