FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Bring in offline packages + config
COPY nuget-packages/ /nuget-packages/
COPY NuGet.config ./
COPY apenew.sln ./
COPY apenew.csproj ./

# Offline restore using ONLY local cache
RUN dotnet restore apenew.csproj \
    --configfile NuGet.config \
    --packages /nuget-packages \
    --verbosity minimal

# Bring in the rest of the source
COPY . .

# Publish without hitting NuGet
RUN dotnet publish apenew.csproj -c Release -o /app/out --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .
# copy YAML config file into /app
COPY appsettings.yaml .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "apenew.dll"]

