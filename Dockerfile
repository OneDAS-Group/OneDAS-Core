# https://hub.docker.com/_/microsoft-dotnet-sdk
# https://github.com/dotnet/dotnet-docker/blob/master/src/sdk/5.0/buster-slim/amd64/Dockerfile
# = 5.0.101-buster-slim-amd64 (Debian 10)

# use patch version because of certifcate issue, otherwise just 5.0 (https://github.com/NuGet/Home/issues/10491)
FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim AS build
WORKDIR /source

# copy everything else and build app
COPY . .
WORKDIR /source/src/OneDas.DataManagement.Explorer
ENV PATH="/root/.dotnet/tools:${PATH}" 
RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN libman restore
ENV NUGET_XMLDOC_MODE=none
RUN rm -rf /root/.nuget/packages/microsoft.netcore.app.ref \
	&& dotnet publish -c release -o /app -r linux-x64 --self-contained false

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["./OneDas.DataManagement.Explorer"]